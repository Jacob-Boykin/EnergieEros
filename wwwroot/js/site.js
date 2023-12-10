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

async function sendMessageToChatAPI(message) {
    try {
        console.log(JSON.stringify({ content: message }))
        const response = await fetch('/api/chat', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ content: message })
        });
        return response.json();
    } catch (error) {
        console.error('Error:', error);
        throw error;
    }
}


// Fetch products from the API endpoint
async function fetchProducts() {
    try {
        const response = await fetch('/api/products');
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        return await response.json();
    } catch (error) {
        console.error('Error fetching products:', error);
        return [];
    }
}

// Function to generate product HTML
function generateProductHTML(product) {
    return `
        <div class="product">
            <img src="${product.imageUrl}" alt="${product.name}">
            <h2>${product.name}</h2>
            <p>${product.description}</p>
            <span class="price">${product.price}</span>
            <button class="nav-link text-dark add-to-cart-btn"
                    data-product-id="${product.productId}"
                    data-name="${product.name}"
                    data-description="${product.description}"
                    data-price="${product.price}"
                    data-quantity="1">
                Add to Cart
            </button>
        </div>
    `;
}

// Function to render products on the page
async function renderProducts() {
    const productContainer = document.getElementById('productContainer');
    const products = await fetchProducts();

    if (products.length === 0) {
        productContainer.innerHTML = '<p>No products available</p>';
        return;
    }

    productContainer.innerHTML = products.map(generateProductHTML).join('');
}