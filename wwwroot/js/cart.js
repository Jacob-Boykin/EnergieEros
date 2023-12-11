// On load, fetch the cart items and display them
window.addEventListener('load', function () {
    fetchCartItems();
    fetchTotal();
});

async function fetchCartItems() {
    try {
        const UserId = await fetchUserId();
        console.log("Fetching cart items for user:", UserId); // Log the UserId
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
    cartItemsDiv.innerHTML = '';

    if (Array.isArray(data) && data.length === 0) {
        cartItemsDiv.innerHTML = '<p>No items in cart. Go shopping!</p>';
        return;
    }

    for (const item of data) {
        try {
            const product = await fetchProductById(item.productId);
            const itemElement = document.createElement('div');
            itemElement.className = 'cart-item';
            itemElement.innerHTML = `
                <p>${product.name} - $${product.price} - Qty: ${item.quantity}</p>
                <button class="delete-cart-item-btn" data-cart-item-id="${item.id}">Delete</button>`;
            cartItemsDiv.appendChild(itemElement);
        } catch (error) {
            console.error('Error fetching product details:', error);
        }
    }

    // Attach event listener for delete buttons
    const deleteButtons = cartItemsDiv.querySelectorAll('.delete-cart-item-btn');
    deleteButtons.forEach(button => {
        button.addEventListener('click', function () {
            const cartItemId = this.dataset.cartItemId;
            deleteCartItem(cartItemId);
        });
    });
}

async function deleteCartItem(cartItemId) {
    try {
        console.log("Deleting cart item with ID:", cartItemId);
        const response = await fetch(`/api/cart/remove/${cartItemId}`, {
            method: 'DELETE'
        });

        if (!response.ok) {
            throw new Error('Failed to delete cart item');
        }

        console.log("Cart item deleted successfully");
        // Refresh cart items and total
        fetchCartItems();
        fetchTotal();
    } catch (error) {
        console.error('Error deleting cart item:', error);
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

        // Update the total display
        updateTotalDisplay(total);
    } catch (error) {
        console.error('Error fetching total:', error);

        // If there's an error (such as no items in the cart), set total to $0.00
        updateTotalDisplay(0);
    }
}

function updateTotalDisplay(total) {
    const totalSpan = document.getElementById('total');
    totalSpan.innerText = `$${total.toFixed(2)}`; // Formats the total to 2 decimal places
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

    // Always add a new item to the cart
    const cartItem = {
        productId: productId,
        quantity: quantity,
        UserId: userId
    };

    await addToDatabase(cartItem);

    fetchCartItems();
    fetchTotal();
}

async function findCartItemByProductId(productId, userId) {
    const response = await fetch(`/api/cart/items/${userId}`);
    if (!response.ok) return null;
    const cartItems = await response.json();
    return cartItems.find(item => item.productId === productId);
}

async function updateCartItem(cartItem) {
try {
        console.log("Updating cart item:", JSON.stringify(cartItem));
        const response = await fetch(`/api/cart/update/${cartItem.id}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(cartItem),
        });

        if (!response.ok) {
            const errorMessage = await response.text();
            console.error('Error updating cart item:', errorMessage);
            throw new Error(errorMessage);
        } else {
            console.log('Cart item updated successfully');
        }
    } catch (error) {
        console.error('Error updating cart item:', error);
    }
}

async function addToDatabase(cartItem) {
    try {
        console.log("Adding to database:", JSON.stringify(cartItem));
        const response = await fetch('/api/cart/add/' + cartItem.UserId, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(cartItem),
        });

        if (!response.ok) {
            const errorMessage = await response.text();
            console.error('Error adding product to the database:', errorMessage);
            throw new Error(errorMessage);
        } else {
            console.log('Product added to the database');
            // Optionally, refresh cart items
            fetchCartItems();
            fetchTotal();
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
    const userId = await fetchUserId();
    try {
        const response = await fetch(`/api/cart/checkout/${userId}`, {
            method: 'POST'
        });

        if (response.ok) {
            console.log('Checkout successful');
            // Clear cart items from the frontend display
            clearCartDisplay();
            // Optionally, send a request to the backend to clear the cart
            await clearCartBackend(userId);
        } else {
            console.error('Checkout failed:', response.statusText);
            // Handle failed checkout scenario
        }
    } catch (error) {
        console.error('Error during checkout:', error);
    }
}

// Function to clear cart items from the frontend display
function clearCartDisplay() {
    const cartItemsDiv = document.getElementById('cart-items');
    if (cartItemsDiv) {
        cartItemsDiv.innerHTML = '<p>Your cart is now empty.</p>';
    }
    updateTotalDisplay(0); // Update the total to $0.00
}

// Function to send a request to the backend to clear the cart
async function clearCartBackend(userId) {
    try {
        const response = await fetch(`/api/cart/clear/${userId}`, {
            method: 'DELETE'
        });

        if (!response.ok) {
            throw new Error('Failed to clear cart in backend');
        }

        console.log('Cart cleared successfully in backend');
    } catch (error) {
        console.error('Error clearing cart in backend:', error);
    }
}

// Function to update the total display
function updateTotalDisplay(total) {
    const totalSpan = document.getElementById('total');
    totalSpan.innerText = `$${total.toFixed(2)}`;
}

