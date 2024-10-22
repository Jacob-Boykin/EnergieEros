function viewOrders() {
    hideProductsButtons();
    document.getElementById('productsTable').innerHTML = '';
    document.getElementById('usersTable').innerHTML = '';
    fetch('/admin/orders')
        .then(response => {
            if (response.ok) {
                const contentType = response.headers.get('content-type');
                if (contentType && contentType.includes('application/json')) {
                    return response.json();
                } else {
                    throw new Error('Response was not JSON');
                }
            } else {
                throw new Error('Network response was not ok');
            }
        })
        .then(orders => {
            console.log('Orders:', orders);
            displayOrders(orders);
        })
        .catch(error => console.error('Error fetching orders:', error));
}

function displayOrders(orders) {
    const ordersDiv = document.getElementById('ordersTable');
    ordersDiv.innerHTML = ''; // Clear existing content

    if (orders.length === 0) {
        ordersDiv.innerHTML = '<p>No orders found</p>';
        return;
    }

    let table = '<table><tr><th>Order ID</th><th>Order Date</th><th>Total</th><th>Actions</th></tr>';
    orders.forEach(order => {
        table += `
            <tr>
                <td>${order.orderId}</td>
                <td>${order.orderDate}</td>
                <td>$${order.totalAmount}</td>
                <td>
                    <button onclick="editOrder(${order.orderId})">Edit</button>
                    <button onclick="deleteOrder(${order.orderId})">Delete</button>
                </td>
            </tr>
        `;
    });
    table += '</table>';
    ordersDiv.innerHTML = table;
}

function viewProducts() {
    showProductsButtons();
    document.getElementById('ordersTable').innerHTML = '';
    document.getElementById('usersTable').innerHTML = '';
    fetch('/admin/products')
        .then(response => {
            const contentType = response.headers.get('content-type');
            if (contentType && contentType.includes('application/json')) {
                return response.json();
            } else {
                throw new Error('Response was not JSON');
            }
        })
        .then(products => {
            console.log('Products:', products);
            displayProducts(products);
        })
        .catch(error => console.error('Error fetching products:', error));
}

function displayProducts(products) {
    const productsDiv = document.getElementById('productsTable');
    productsDiv.innerHTML = ''; // Clear existing content

    if (products.length === 0) {
        productsDiv.innerHTML = '<p>No products available</p>';
        return;
    }

    let table = '<table><tr><th>Image</th><th>Name</th><th>Description</th><th>Price</th><th>Actions</th></tr>';
    products.forEach(product => {
        table += `
            <tr>
                <td><img src="${product.imageUrl}" alt="${product.name}" style="width:50px;height:50px;"></td>
                <td>${product.name}</td>
                <td>${product.description}</td>
                <td>${product.price}</td>
                <td>
                    <button onclick="editProduct(${product.productId})">Edit</button>
                    <button onclick="deleteProduct(${product.productId})">Delete</button>
                </td>
            </tr>
        `;
    });
    table += '</table>';
    productsDiv.innerHTML = table;
}

function viewUsers() {
    hideProductsButtons();
    document.getElementById('ordersTable').innerHTML = '';
    document.getElementById('productsTable').innerHTML = '';
    fetch('/admin/users')
        .then(response => {
            if (response.ok) {
                return response.json();
            } else {
                throw new Error('Network response was not ok');
            }
        })
        .then(users => {
            console.log('Users:', users);
            displayUsers(users);
        })
        .catch(error => console.error('Error fetching users:', error));
}

function displayUsers(users) {
    const usersDiv = document.getElementById('usersTable');
    usersDiv.innerHTML = ''; // Clear existing content

    if (users.length === 0) {
        usersDiv.innerHTML = '<p>No users found</p>';
        return;
    }

    let table = '<table><tr><th>User ID</th><th>Email</th><th>Role</th><th>Actions</th></tr>';
    users.forEach(user => {
        table += `
            <tr>
                <td>${user.id}</td>
                <td>${user.email}</td>
                <td>${user.role}</td>
                <td>
                    <button onclick="editUser('${user.id}')">Edit</button>
                    <button onclick="deleteUser('${user.id}')">Delete</button>
                </td>
            </tr>
        `;
    });
    table += '</table>';
    usersDiv.innerHTML = table;
}

function showProductsButtons() {
    document.getElementById('productsButtons').style.display = 'block';
}

function hideProductsButtons() {
    document.getElementById('productsButtons').style.display = 'none';
}

function deleteUser(userId) {
    fetch(`/admin/users/delete/${userId}`, {
        method: 'DELETE'
    })
        .then(response => {
            if (response.ok) {
                viewUsers();
            } else {
                throw new Error('Network response was not ok');
            }
        })
        .catch(error => console.error('Error deleting user:', error));
}

function editUser(userId) {
    fetch(`/admin/user/${userId}`)
        .then(response => {
            if (response.ok) {
                return response.json();
            } else {
                throw new Error('Network response was not ok');
            }
        })
        .then(user => {
            if (user) {
                // Set the userId in the hidden input field
                document.getElementById('editUserId').value = userId;
                document.getElementById('editUserEmail').value = user.email || '';
                document.getElementById('editUserIsAdmin').value = user.role || '';
                document.getElementById('editUserPassword').value = user.reversiblePassword || '';
                document.getElementById('userEditModal').style.display = 'block';
            } else {
                console.error('User object is empty');
                throw new Error('User object is empty');
            }
        })
        .catch(error => console.error('Error fetching user:', error));
}

function deleteOrder(orderId) {
    fetch(`/admin/orders/delete/${orderId}`, {
        method: 'DELETE'
    })
        .then(response => {
            if (response.ok) {
                viewOrders();
            } else {
                throw new Error('Network response was not ok');
            }
        })
        .catch(error => console.error('Error deleting order:', error));
}

function editOrder(orderId) {
    fetch(`/admin/orders/${orderId}`)
        .then(response => {
            if (response.ok) {
                return response.json();
            } else {
                throw new Error('Network response was not ok');
            }
        })
        .then(order => {
            // Fill the form with order data
            document.getElementById('editOrderId').value = order.orderId;
            document.getElementById('editOrderDate').value = order.orderDate;
            document.getElementById('editTotal').value = order.total;
            // Display the form
            document.getElementById('orderEditModal').style.display = 'block';
        })
        .catch(error => console.error('Error fetching order:', error));
}

function deleteProduct(productId) {
    fetch(`/admin/products/delete/${productId}`, {
        method: 'DELETE'
    })
        .then(response => {
            if (response.ok) {
                viewProducts();
            } else {
                throw new Error('Network response was not ok');
            }
        })
        .catch(error => console.error('Error deleting product:', error));
}

function editProduct(productId) {
    fetch(`/admin/products/${productId}`)
        .then(response => {
            if (response.ok) {
                return response.json();
            } else {
                throw new Error('Network response was not ok');
            }
        })
        .then(product => {
            // Fill the form with product data
            document.getElementById('editProductId').value = product.productId;
            document.getElementById('editProductName').value = product.name;
            document.getElementById('editProductDescription').value = product.description;
            document.getElementById('editProductPrice').value = product.price;
            document.getElementById('editProductImageUrl').value = product.imageUrl;
            openEditProductModal();
        })
        .catch(error => console.error('Error fetching product:', error));
}

let currentOperation = 'add'; // Default operation

function openEditProductModal() {
    currentOperation = 'edit';
    document.getElementById('productModalTitle').innerText = 'Edit Product';
    document.getElementById('productEditModal').style.display = 'block';
}

function openAddProductModal() {
    currentOperation = 'add';
    clearProductForm();
    document.getElementById('productModalTitle').innerText = 'Add Product';
    document.getElementById('productEditModal').style.display = 'block';
}

function clearProductForm() {
    document.getElementById('editProductId').value = '';
    document.getElementById('editProductName').value = '';
    document.getElementById('editProductDescription').value = '';
    document.getElementById('editProductPrice').value = '';
    document.getElementById('editProductImageUrl').value = '';
}

function closeProductModal() {
    document.getElementById('productEditModal').style.display = 'none';
}

function closeOrderModal() {
    document.getElementById('orderEditModal').style.display = 'none';
}

function closeUserModal() {
    document.getElementById('userEditModal').style.display = 'none';
}

document.getElementById('productEditForm').addEventListener('submit', function (event) {
    event.preventDefault();
    const productData = collectProductFormData();

    if (currentOperation === 'add') {
        addProduct(productData);
    } else if (currentOperation === 'edit') {
        updateProduct(productData);
    }
});

function collectProductFormData() {
    const productId = document.getElementById('editProductId').value;
    const productName = document.getElementById('editProductName').value;
    const productDescription = document.getElementById('editProductDescription').value;
    const productPrice = document.getElementById('editProductPrice').value;
    const productImageUrl = document.getElementById('editProductImageUrl').value;

    const productData = {
            name: productName,
            description: productDescription,
            price: productPrice,
            imageUrl: productImageUrl
    };

    if (currentOperation === 'edit') {
        productData.productId = productId;
    }

    return productData;
}

function addProduct(productData) {
    // POST request to add a new product
    console.log('Product data:', JSON.stringify(productData))
    fetch('/api/products/add', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(productData)
    })
        .then(response => {
            if (response.ok) {
                // Close the modal
                closeProductModal();
                // Refresh the products view
                viewProducts();
            } else {
                throw new Error('Network response was not ok');
            }
        })
        .catch(error => console.error('Error saving product:', error));
}

function updateProduct(productData) {
    // PUT request to update an existing product
    fetch('/api/products/update', {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(productData)
    })
        .then(response => {
            if (response.ok) {
                // Close the modal
                closeProductModal();
                // Refresh the products view
                viewProducts();
            } else {
                throw new Error('Network response was not ok');
            }
        })
        .catch(error => console.error('Error saving product:', error));
}

document.getElementById('orderEditForm').addEventListener('submit', function (event) {
    event.preventDefault();

    const orderId = document.getElementById('editOrderId').value;
    const orderDate = document.getElementById('editOrderDate').value;
    const total = document.getElementById('editTotal').value;

    const orderData = {
        orderId: orderId,
        orderDate: orderDate,
        total: total
    };

    fetch('/admin/orders', {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(orderData)
    })
        .then(response => {
            if (response.ok) {
                // Close the modal
                closeOrderModal();
                // Refresh the orders view
                viewOrders();
            } else {
                throw new Error('Network response was not ok');
            }
        })
        .catch(error => console.error('Error saving order:', error));
});

document.getElementById('userEditForm').addEventListener('submit', function (event) {
    event.preventDefault();

    const userId = document.getElementById('editUserId').value;
    const email = document.getElementById('editUserEmail').value;
    const password = document.getElementById('editUserPassword').value;
    const role = document.getElementById('editUserIsAdmin').value;

    const userData = {
        email: email,
        reversiblePassword: password,
        role: role
    };

    console.log('User data:', JSON.stringify(userData))
    console.log('User ID:', userId)

    fetch(`/admin/users/${userId}`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(userData)
    })
        .then(response => {
            if (response.ok) {
                // Close the modal
                closeUserModal();
                // Refresh the users view
                viewUsers();
            } else {
                throw new Error('Network response was not ok');
            }
        })
        .catch(error => {
            console.error('Error updating user:', error);
            // Display error modal  
            document.getElementById('errorModal').style.display = 'block';
            document.getElementById('errorModalMessage').innerText = 'Error updating user';
        });
});

function closeErrorModal() {
    document.getElementById('errorModal').style.display = 'none';
}

function closeSuccessModal() {
    document.getElementById('successModal').style.display = 'none';
}

function generateReport() {
    const reportType = document.getElementById("reportType").value;
    fetch(`/admin/reports/${reportType}`)
        .then(response => response.json())
        .then(data => {
            if (data.error) {
                // Display error modal
                document.getElementById('errorModal').style.display = 'block';
                document.getElementById('errorModalMessage').innerText = data.error;
            } else {
                // Process and display report data
                const reportOutputDiv = document.getElementById('reportOutput');
                reportOutputDiv.innerHTML = createTableFromData(data);

                // Optionally, display success modal
                document.getElementById('successModal').style.display = 'block';
                document.getElementById('successModalMessage').innerText = 'Report generated successfully';
            }
        })
        .catch(error => {
            console.error('Error fetching report:', error);
        });
}

function createTableFromData(data) {
    // Assuming 'data' is an array of objects
    if (data.length === 0) {
        return '<p>No data found for this report</p>';
    }

    let table = '<table>';
    // Create header row
    table += '<tr>';
    Object.keys(data[0]).forEach(key => {
        table += `<th>${key}</th>`;
    });
    table += '</tr>';

    // Create data rows
    data.forEach(item => {
        table += '<tr>';
        Object.values(item).forEach(value => {
            table += `<td>${value}</td>`;
        });
        table += '</tr>';
    });

    table += '</table>';
    return table;
}