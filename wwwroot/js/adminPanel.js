function viewOrders() {
    hideProductsButtons();
    fetch('/api/orders')
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

    console.log('Orders:', orders);

    ordersDiv.innerHTML = orders.map(order => {
        return `
            <div class="order">
                <h2>Order #${order.orderId}</h2>
                <p>Order Date: ${order.orderDate}</p>
                <p>Order Total: ${order.total}</p>
                <button onclick="editOrder(${order.orderId})">Edit</button>
                <button onclick="deleteOrder(${order.orderId})">Delete</button>
            </div>
        `;
    }).join('');
}

function viewProducts() {
    showProductsButtons();
    fetch('/api/products')
        .then(response => {
            if (response.ok) {
                return response.json();
            } else {
                throw new Error('Network response was not ok');
            }
        })
        .then(products => {
            console.log('Products:', products);
            displayProducts(products);
        })
        .catch(error => console.error('Error fetching products:', error));
}

function displayProducts(products) {
    const productsDiv = document.getElementById('products');
    productsDiv.innerHTML = ''; // Clear existing content

    if (products.length === 0) {
        productsDiv.innerHTML = '<p>No products available</p>';
        return;
    }

    console.log('Products:', products);

    productsDiv.innerHTML = products.map(product => {
        return `
            <div class="product">
                <img src="${product.imageUrl}" alt="${product.name}">
                <h2>${product.name}</h2>
                <p>${product.description}</p>
                <span class="price">${product.price}</span>
            </div>
        `;
    }).join('');
}

function viewUsers() {
    hideProductsButtons();
    fetch('/api/users')
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
    const usersDiv = document.getElementById('users');
    usersDiv.innerHTML = ''; // Clear existing content

    if (users.length === 0) {
        usersDiv.innerHTML = '<p>No users found</p>';
        return;
    }

    console.log('Users:', users);

    usersDiv.innerHTML = users.map(user => {
        return `
            <div class="user">
                <h2>${user.id}</h2>
                <p>${user.email}</p>
                <button onclick="editUser(${user.userId})">Edit</button>
                <button onclick="deleteUser(${user.userId})">Delete</button>
            </div>
        `;
    }).join('');
}

function showProductsButtons() {
    document.getElementById('productsButtons').style.display = 'block';
}

function hideProductsButtons() {
    document.getElementById('productsButtons').style.display = 'none';
}

function deleteUser(userId) {
    fetch(`/api/users/delete/${userId}`, {
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
    fetch(`/api/users/${userId}`)
        .then(response => {
            if (response.ok) {
                return response.json();
            } else {
                throw new Error('Network response was not ok');
            }
        })
        .then(user => {
            console.log('User:', user);
            displayUserForm(user);
        })
        .catch(error => console.error('Error fetching user:', error));
}

function deleteOrder(orderId) {
    fetch(`/api/orders/delete/${orderId}`, {
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
    fetch(`/api/orders/${orderId}`)
        .then(response => {
            if (response.ok) {
                return response.json();
            } else {
                throw new Error('Network response was not ok');
            }
        })
        .then(order => {
            console.log('Order:', order);
            displayOrderForm(order);
        })
        .catch(error => console.error('Error fetching order:', error));
}

function deleteProduct(productId) {
    fetch(`/api/products/delete/${productId}`, {
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
    fetch(`/api/products/${productId}`)
        .then(response => {
            if (response.ok) {
                return response.json();
            } else {
                throw new Error('Network response was not ok');
            }
        })
        .then(product => {
            console.log('Product:', product);
            displayProductForm(product);
        })
        .catch(error => console.error('Error fetching product:', error));
}

function addProduct() {
    displayProductForm();
}

function displayProductForm(product) {
    const productFormDiv = document.getElementById('productForm');
    productFormDiv.innerHTML = `
        <h2>Product</h2>
        <form onsubmit="saveProduct(event)">
            <input type="hidden" id="productId" name="productId" value="${product ? product.productId : ''}" />
            <label for="name">Name</label>
            <input type="text" id="name" name="name" value="${product ? product.name : ''}" required />
            <label for="description">Description</label>
            <input type="text" id="description" name="description" value="${product ? product.description : ''}" required />
            <label for="price">Price</label>
            <input type="number" id="price" name="price" value="${product ? product.price : ''}" required />
            <label for="imageUrl">Image URL</label>
            <input type="text" id="imageUrl" name="imageUrl" value="${product ? product.imageUrl : ''}" required />
            <input type="submit" value="Save" />
        </form>
    `;
}

function saveProduct(event) {
    event.preventDefault();

    const productId = document.getElementById('productId').value;
    const name = document.getElementById('name').value;
    const description = document.getElementById('description').value;
    const price = document.getElementById('price').value;
    const imageUrl = document.getElementById('imageUrl').value;

    const product = {
        productId: productId,
        name: name,
        description: description,
        price: price,
        imageUrl: imageUrl
    };

    fetch('/api/products', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(product)
    })
        .then(response => {
            if (response.ok) {
                viewProducts();
            } else {
                throw new Error('Network response was not ok');
            }
        })
        .catch(error => console.error('Error saving product:', error));
}

function displayUserForm(user) {
    const userFormDiv = document.getElementById('userForm');
    userFormDiv.innerHTML = `
        <h2>User</h2>
        <form onsubmit="saveUser(event)">
            <input type="hidden" id="userId" name="userId" value="${user ? user.userId : ''}" />
            <label for="firstName">First Name</label>
            <input type="text" id="firstName" name="firstName" value="${user ? user.firstName : ''}" required />
            <label for="lastName">Last Name</label>
            <input type="text" id="lastName" name="lastName" value="${user ? user.lastName : ''}" required />
            <label for="email">Email</label>
            <input type="text" id="email" name="email" value="${user ? user.email : ''}" required />
            <input type="submit" value="Save" />
        </form>
    `;
}

function saveUser(event) {
    event.preventDefault();

    const userId = document.getElementById('userId').value;
    const firstName = document.getElementById('firstName').value;
    const lastName = document.getElementById('lastName').value;
    const email = document.getElementById('email').value;

    const user = {
        userId: userId,
        firstName: firstName,
        lastName: lastName,
        email: email
    };

    fetch('/api/users', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(user)
    })
        .then(response => {
            if (response.ok) {
                viewUsers();
            } else {
                throw new Error('Network response was not ok');
            }
        })
        .catch(error => console.error('Error saving user:', error));
}

function displayOrderForm(order) {
    const orderFormDiv = document.getElementById('orderForm');
    orderFormDiv.innerHTML = `
        <h2>Order</h2>
        <form onsubmit="saveOrder(event)">
            <input type="hidden" id="orderId" name="orderId" value="${order ? order.orderId : ''}" />
            <label for="orderDate">Order Date</label>
            <input type="text" id="orderDate" name="orderDate" value="${order ? order.orderDate : ''}" required />
            <label for="total">Total</label>
            <input type="text" id="total" name="total" value="${order ? order.total : ''}" required />
            <input type="submit" value="Save" />
        </form>
    `;
}

function saveOrder(event) {
    event.preventDefault();

    const orderId = document.getElementById('orderId').value;
    const orderDate = document.getElementById('orderDate').value;
    const total = document.getElementById('total').value;

    const order = {
        orderId: orderId,
        orderDate: orderDate,
        total: total
    };

    fetch('/api/orders', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(order)
    })
        .then(response => {
            if (response.ok) {
                viewOrders();
            } else {
                throw new Error('Network response was not ok');
            }
        })
        .catch(error => console.error('Error saving order:', error));
}
