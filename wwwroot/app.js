// API Configuration
const API_URL = 'http://localhost:5000/api';
let token = localStorage.getItem('token');
let currentUser = null;

// Utility Functions
function showLoading() {
    document.getElementById('loadingOverlay').classList.remove('hidden');
}

function hideLoading() {
    document.getElementById('loadingOverlay').classList.add('hidden');
}

function showToast(message, type = 'info') {
    const toast = document.getElementById('toast');
    toast.textContent = message;
    toast.className = `toast ${type}`;

    setTimeout(() => {
        toast.classList.add('hidden');
    }, 3000);
}

function formatCurrency(amount) {
    return new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD'
    }).format(amount);
}

function formatDate(dateString) {
    return new Date(dateString).toLocaleDateString('es-ES');
}

// API Request Helper
async function apiRequest(endpoint, options = {}) {
    showLoading();

    const config = {
        ...options,
        headers: {
            'Content-Type': 'application/json',
            ...(token && { 'Authorization': `Bearer ${token}` }),
            ...options.headers
        }
    };

    try {
        const response = await fetch(`${API_URL}${endpoint}`, config);
        const data = await response.json();

        if (!response.ok) {
            throw new Error(data.message || 'Request failed');
        }

        hideLoading();
        return data;
    } catch (error) {
        hideLoading();
        showToast(error.message, 'error');
        throw error;
    }
}

// Authentication
async function login(username, password) {
    try {
        const data = await apiRequest('/auth/login', {
            method: 'POST',
            body: JSON.stringify({ username, password })
        });

        token = data.token;
        currentUser = data;
        localStorage.setItem('token', token);
        localStorage.setItem('user', JSON.stringify(data));

        updateUI();
        showToast('Login successful!', 'success');
        switchView('dashboard');
        loadDashboard();
    } catch (error) {
        console.error('Login error:', error);
    }
}

function logout() {
    token = null;
    currentUser = null;
    localStorage.removeItem('token');
    localStorage.removeItem('user');

    updateUI();
    switchView('login');
    showToast('Logged out successfully', 'info');
}

function updateUI() {
    const navItems = document.querySelectorAll('.nav-item');
    const logoutBtn = document.getElementById('logoutBtn');
    const userInfo = document.getElementById('userInfo');

    if (token) {
        navItems.forEach(item => item.classList.remove('disabled'));
        logoutBtn.classList.remove('hidden');

        const user = JSON.parse(localStorage.getItem('user'));
        userInfo.innerHTML = `
            <span class="user-name">${user.username}</span>
            <span class="user-role badge">${user.role}</span>
        `;
    } else {
        navItems.forEach(item => {
            if (item.dataset.view !== 'login') {
                item.classList.add('disabled');
            }
        });
        logoutBtn.classList.add('hidden');
        userInfo.innerHTML = `
            <span class="user-name">Guest</span>
            <span class="user-role badge">Not Logged</span>
        `;
    }
}

// View Management
function switchView(viewName) {
    // Update active nav
    document.querySelectorAll('.nav-item').forEach(item => {
        item.classList.remove('active');
        if (item.dataset.view === viewName) {
            item.classList.add('active');
        }
    });

    // Update active view
    document.querySelectorAll('.view').forEach(view => {
        view.classList.remove('active');
    });
    document.getElementById(`${viewName}View`).classList.add('active');

    // Load data for the view
    switch (viewName) {
        case 'dashboard':
            loadDashboard();
            break;
        case 'departments':
            loadDepartments();
            break;
        case 'employees':
            loadEmployees();
            break;
        case 'dependents':
            loadDependents();
            break;
    }
}

// Dashboard
async function loadDashboard() {
    try {
        const [depts, emps, deps, empStats] = await Promise.all([
            apiRequest('/departments'),
            apiRequest('/employees'),
            apiRequest('/dependents'),
            apiRequest('/employees/stats')
        ]);

        document.getElementById('totalDepartments').textContent = depts.length;
        document.getElementById('totalEmployees').textContent = emps.length;
        document.getElementById('totalDependents').textContent = deps.length;
        document.getElementById('avgSalary').textContent = formatCurrency(empStats.averageSalary || 0);

        const quickStats = document.getElementById('quickStats');
        quickStats.innerHTML = `
            <p><strong>Total Departments:</strong> ${depts.length}</p>
            <p><strong>Total Employees:</strong> ${emps.length}</p>
            <p><strong>Total Dependents:</strong> ${deps.length}</p>
            <p><strong>Average Salary:</strong> ${formatCurrency(empStats.averageSalary || 0)}</p>
        `;
    } catch (error) {
        console.error('Dashboard error:', error);
    }
}

// Departments
async function loadDepartments() {
    try {
        const departments = await apiRequest('/departments');
        const tbody = document.getElementById('departmentsTableBody');

        if (departments.length === 0) {
            tbody.innerHTML = '<tr><td colspan="5" class="text-center">No departments found</td></tr>';
            return;
        }

        tbody.innerHTML = departments.map(dept => `
            <tr>
                <td><strong>${dept.code}</strong></td>
                <td>${dept.name}</td>
                <td>${dept.location || 'N/A'}</td>
                <td>${dept.employeeCount || 0}</td>
                <td>
                    <button class="btn btn-sm btn-primary" onclick="editDepartment(${dept.id})">✏️ Edit</button>
                    <button class="btn btn-sm btn-danger" onclick="deleteDepartment(${dept.id})">🗑️ Delete</button>
                </td>
            </tr>
        `).join('');
    } catch (error) {
        console.error('Load departments error:', error);
    }
}

function openDepartmentModal(id = null) {
    const modal = document.getElementById('departmentModal');
    const form = document.getElementById('departmentForm');

    form.reset();
    document.getElementById('departmentId').value = '';
    document.getElementById('departmentModalTitle').textContent = 'New Department';

    if (id) {
        // Load department data for editing
        apiRequest(`/departments/${id}`).then(dept => {
            document.getElementById('departmentId').value = dept.id;
            document.getElementById('deptName').value = dept.name;
            document.getElementById('deptCode').value = dept.code;
            document.getElementById('deptDescription').value = dept.description || '';
            document.getElementById('deptLocation').value = dept.location || '';
            document.getElementById('departmentModalTitle').textContent = 'Edit Department';
        });
    }

    modal.classList.remove('hidden');
}

function closeDepartmentModal() {
    document.getElementById('departmentModal').classList.add('hidden');
}

async function editDepartment(id) {
    openDepartmentModal(id);
}

async function deleteDepartment(id) {
    if (!confirm('Are you sure you want to delete this department?')) return;

    try {
        await apiRequest(`/departments/${id}`, { method: 'DELETE' });
        showToast('Department deleted successfully', 'success');
        loadDepartments();
    } catch (error) {
        console.error('Delete department error:', error);
    }
}

// Employees
async function loadEmployees() {
    try {
        const employees = await apiRequest('/employees');
        const tbody = document.getElementById('employeesTableBody');

        if (employees.length === 0) {
            tbody.innerHTML = '<tr><td colspan="6" class="text-center">No employees found</td></tr>';
            return;
        }

        tbody.innerHTML = employees.map(emp => `
            <tr>
                <td><strong>${emp.fullName}</strong></td>
                <td>${emp.email}</td>
                <td>${emp.position || 'N/A'}</td>
                <td>${emp.department?.name || 'N/A'}</td>
                <td>${formatCurrency(emp.salary)}</td>
                <td>
                    <button class="btn btn-sm btn-primary" onclick="editEmployee(${emp.id})">✏️ Edit</button>
                    <button class="btn btn-sm btn-danger" onclick="deleteEmployee(${emp.id})">🗑️ Delete</button>
                </td>
            </tr>
        `).join('');
    } catch (error) {
        console.error('Load employees error:', error);
    }
}

async function openEmployeeModal(id = null) {
    const modal = document.getElementById('employeeModal');
    const form = document.getElementById('employeeForm');

    // Load departments for dropdown
    const departments = await apiRequest('/departments');
    const deptSelect = document.getElementById('empDepartment');
    deptSelect.innerHTML = '<option value="">Select Department</option>' +
        departments.map(d => `<option value="${d.id}">${d.name}</option>`).join('');

    form.reset();
    document.getElementById('employeeId').value = '';
    document.getElementById('employeeModalTitle').textContent = 'New Employee';

    if (id) {
        const emp = await apiRequest(`/employees/${id}`);
        document.getElementById('employeeId').value = emp.id;
        document.getElementById('empFirstName').value = emp.firstName;
        document.getElementById('empLastName').value = emp.lastName;
        document.getElementById('empEmail').value = emp.email;
        document.getElementById('empPhone').value = emp.phoneNumber || '';
        document.getElementById('empHireDate').value = emp.hireDate.split('T')[0];
        document.getElementById('empPosition').value = emp.position || '';
        document.getElementById('empSalary').value = emp.salary;
        document.getElementById('empDepartment').value = emp.departmentId || '';
        document.getElementById('employeeModalTitle').textContent = 'Edit Employee';
    }

    modal.classList.remove('hidden');
}

function closeEmployeeModal() {
    document.getElementById('employeeModal').classList.add('hidden');
}

async function editEmployee(id) {
    openEmployeeModal(id);
}

async function deleteEmployee(id) {
    if (!confirm('Are you sure you want to delete this employee?')) return;

    try {
        await apiRequest(`/employees/${id}`, { method: 'DELETE' });
        showToast('Employee deleted successfully', 'success');
        loadEmployees();
    } catch (error) {
        console.error('Delete employee error:', error);
    }
}

// Dependents
async function loadDependents() {
    try {
        const dependents = await apiRequest('/dependents');
        const tbody = document.getElementById('dependentsTableBody');

        if (dependents.length === 0) {
            tbody.innerHTML = '<tr><td colspan="5" class="text-center">No dependents found</td></tr>';
            return;
        }

        tbody.innerHTML = dependents.map(dep => `
            <tr>
                <td><strong>${dep.fullName}</strong></td>
                <td>${dep.employeeName}</td>
                <td>${dep.relationship}</td>
                <td>${dep.age} years</td>
                <td>
                    <button class="btn btn-sm btn-primary" onclick="editDependent(${dep.id})">✏️ Edit</button>
                    <button class="btn btn-sm btn-danger" onclick="deleteDependent(${dep.id})">🗑️ Delete</button>
                </td>
            </tr>
        `).join('');
    } catch (error) {
        console.error('Load dependents error:', error);
    }
}

async function openDependentModal(id = null) {
    const modal = document.getElementById('dependentModal');
    const form = document.getElementById('dependentForm');

    // Load employees for dropdown
    const employees = await apiRequest('/employees');
    const empSelect = document.getElementById('depEmployee');
    empSelect.innerHTML = '<option value="">Select Employee</option>' +
        employees.map(e => `<option value="${e.id}">${e.fullName}</option>`).join('');

    form.reset();
    document.getElementById('dependentId').value = '';
    document.getElementById('dependentModalTitle').textContent = 'New Dependent';

    if (id) {
        const dep = await apiRequest(`/dependents/${id}`);
        document.getElementById('dependentId').value = dep.id;
        document.getElementById('depFirstName').value = dep.firstName;
        document.getElementById('depLastName').value = dep.lastName;
        document.getElementById('depDateOfBirth').value = dep.dateOfBirth.split('T')[0];
        document.getElementById('depRelationship').value = dep.relationship;
        document.getElementById('depGender').value = dep.gender || '';
        document.getElementById('depIdNumber').value = dep.identificationNumber || '';
        document.getElementById('depEmployee').value = dep.employeeId;
        document.getElementById('dependentModalTitle').textContent = 'Edit Dependent';
    }

    modal.classList.remove('hidden');
}

function closeDependentModal() {
    document.getElementById('dependentModal').classList.add('hidden');
}

async function editDependent(id) {
    openDependentModal(id);
}

async function deleteDependent(id) {
    if (!confirm('Are you sure you want to delete this dependent?')) return;

    try {
        await apiRequest(`/dependents/${id}`, { method: 'DELETE' });
        showToast('Dependent deleted successfully', 'success');
        loadDependents();
    } catch (error) {
        console.error('Delete dependent error:', error);
    }
}

// Event Listeners
document.addEventListener('DOMContentLoaded', () => {
    // Check if already logged in
    if (token) {
        currentUser = JSON.parse(localStorage.getItem('user'));
        updateUI();
        switchView('dashboard');
    }

    // Login Form
    document.getElementById('loginForm').addEventListener('submit', (e) => {
        e.preventDefault();
        const username = document.getElementById('username').value;
        const password = document.getElementById('password').value;
        login(username, password);
    });

    // Logout Button
    document.getElementById('logoutBtn').addEventListener('click', logout);

    // Navigation
    document.querySelectorAll('.nav-item').forEach(item => {
        item.addEventListener('click', (e) => {
            e.preventDefault();
            if (!item.classList.contains('disabled')) {
                switchView(item.dataset.view);
            }
        });
    });

    // Department Form
    document.getElementById('departmentForm').addEventListener('submit', async (e) => {
        e.preventDefault();

        const id = document.getElementById('departmentId').value;
        const data = {
            name: document.getElementById('deptName').value,
            code: document.getElementById('deptCode').value,
            description: document.getElementById('deptDescription').value,
            location: document.getElementById('deptLocation').value
        };

        try {
            if (id) {
                await apiRequest(`/departments/${id}`, {
                    method: 'PUT',
                    body: JSON.stringify(data)
                });
                showToast('Department updated successfully', 'success');
            } else {
                await apiRequest('/departments', {
                    method: 'POST',
                    body: JSON.stringify(data)
                });
                showToast('Department created successfully', 'success');
            }

            closeDepartmentModal();
            loadDepartments();
        } catch (error) {
            console.error('Save department error:', error);
        }
    });

    // Employee Form
    document.getElementById('employeeForm').addEventListener('submit', async (e) => {
        e.preventDefault();

        const id = document.getElementById('employeeId').value;
        const data = {
            firstName: document.getElementById('empFirstName').value,
            lastName: document.getElementById('empLastName').value,
            email: document.getElementById('empEmail').value,
            phoneNumber: document.getElementById('empPhone').value,
            hireDate: document.getElementById('empHireDate').value,
            position: document.getElementById('empPosition').value,
            salary: parseFloat(document.getElementById('empSalary').value),
            departmentId: parseInt(document.getElementById('empDepartment').value) || null
        };

        try {
            if (id) {
                await apiRequest(`/employees/${id}`, {
                    method: 'PUT',
                    body: JSON.stringify(data)
                });
                showToast('Employee updated successfully', 'success');
            } else {
                await apiRequest('/employees', {
                    method: 'POST',
                    body: JSON.stringify(data)
                });
                showToast('Employee created successfully', 'success');
            }

            closeEmployeeModal();
            loadEmployees();
        } catch (error) {
            console.error('Save employee error:', error);
        }
    });

    // Dependent Form
    document.getElementById('dependentForm').addEventListener('submit', async (e) => {
        e.preventDefault();

        const id = document.getElementById('dependentId').value;
        const data = {
            firstName: document.getElementById('depFirstName').value,
            lastName: document.getElementById('depLastName').value,
            dateOfBirth: document.getElementById('depDateOfBirth').value,
            relationship: document.getElementById('depRelationship').value,
            gender: document.getElementById('depGender').value,
            identificationNumber: document.getElementById('depIdNumber').value,
            employeeId: parseInt(document.getElementById('depEmployee').value)
        };

        try {
            if (id) {
                await apiRequest(`/dependents/${id}`, {
                    method: 'PUT',
                    body: JSON.stringify(data)
                });
                showToast('Dependent updated successfully', 'success');
            } else {
                await apiRequest('/dependents', {
                    method: 'POST',
                    body: JSON.stringify(data)
                });
                showToast('Dependent created successfully', 'success');
            }

            closeDependentModal();
            loadDependents();
        } catch (error) {
            console.error('Save dependent error:', error);
        }
    });

    // Close modals on outside click
    document.querySelectorAll('.modal').forEach(modal => {
        modal.addEventListener('click', (e) => {
            if (e.target === modal) {
                modal.classList.add('hidden');
            }
        });
    });
});