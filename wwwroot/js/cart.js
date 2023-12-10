async function fetchCartItems() {
    try {
        const UserId = await fetchUserId();
        const response = await fetch(`/api/cart/items/${UserId}`);
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        const data = await response.json();
        displayCartItems(data);
    } catch (error) {
        console.error('Error fetching cart items:', error);
    }
}

async function displayCartItems(data) {
    const cartItemsDiv = document.getElementById('cart-items');
    cartItemsDiv.innerHTML = ''; // Clear existing content

    if (data.length === 0) {
        cartItemsDiv.innerHTML = '<p>No items in cart</p>';
        return;
    }

    console.log('Cart items:', data);

    // Fetch product details for each item in the cart in parallel
    for (const item of data) {
        try {
            const product = await fetchProductById(item.productId);
            const itemElement = document.createElement('div');
            itemElement.innerHTML = `<p>${product.name} - $${product.price}</p>`;
            cartItemsDiv.appendChild(itemElement);
        } catch (error) {
            console.error('Error fetching product details:', error);
        }
    }
}


async function fetchProductById(id) {
    try {
        const response = await fetch(`/api/products/${id}`);
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        const data = await response.json();
        return data;
    } catch (error) {
        console.error('Error fetching product:', error);
        throw error;
    }
}

async function fetchUserId() {
    try {
        const response = await fetch('/api/user/id');
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        const data = await response.text(); // Get the response as text

        //console.log('Received Data:', data); // Log the received data to inspect it

        // Parse the user ID from the response string
        const userIdMatch = data.match(/User ID: (\S+)/); // Updated regex to match any non-whitespace characters
        if (userIdMatch && userIdMatch.length > 1) {
            const userId = userIdMatch[1]; // Extracted user ID
            console.log('User ID:', userId);
            return userId;
        } else {
            console.error('User ID not found in the response');
            throw new Error('User ID not found');
        }
    } catch (error) {
        console.error('Error fetching user ID:', error);
        throw error;
    }
}

async function fetchTotal() {
    try {
        const UserId = await fetchUserId();
        const response = await fetch(`/api/cart/total/${UserId}`);
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        const total = await response.json();
        return total;
    } catch (error) {
        console.error('Error fetching total:', error);
        throw error;
    }
}

addToCartButtons = document.querySelectorAll('.add-to-cart-btn');

addToCartButtons.forEach(button => {
    button.addEventListener('click', function () {
        const productId = this.dataset.productId;
        const quantity = 1;

        addToCart(productId, quantity);
    });
});

async function addToCart(productId, quantity) {
    const userId = await fetchUserId();

    const cartItem = {
        productId: productId,
        quantity: quantity,
        UserId: userId
    };

    addToDatabase(cartItem);
    console.log('Added to cart:', cartItem);
}

async function addToDatabase(cartItem) {
    try {
        console.log(JSON.stringify(cartItem));
        const response = await fetch('/api/cart/add/{cartItem.userId}', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(cartItem),
        });

        if (response.ok) {
            console.log('Product added to the database');
        } else {
            const errorMessage = await response.text();
            throw new Error(errorMessage);
        }
    } catch (error) {
        console.error('Error adding product to the database:', error);
    }
}

async function getOrderById(id) {
    try {
        const response = await fetch(`/api/orders/${id}`);
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        const data = await response.json();
        return data;
    } catch (error) {
        console.error('Error fetching order:', error);
        throw error;
    }
}

async function checkout() {
    fetch(`/api/cart/checkout/${id}`, {
        method: 'GET' // Assuming it's a GET request for checkout
        // Add headers and body if needed
    })
        .then(response => {
            if (response.ok) {
                console.log('Checkout successful');
                // Perform any necessary actions after successful checkout
            } else {
                console.error('Checkout failed');
                // Handle failed checkout scenario
            }
        })
        .catch(error => console.error('Error during checkout:', error));
}