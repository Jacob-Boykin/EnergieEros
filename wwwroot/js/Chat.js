const express = require('express');
const axios = require('axios');

const app = express();
const PORT = 3000;

app.use(express.json());

app.post('/chat', async (req, res) => {
    try {
        const { message } = req.body; // Assuming the frontend sends user message in the request body

        // Call OpenAI API
        const response = await axios.post('https://api.openai.com/v1/engines/davinci/completions', {
            prompt: message,
            // Add your API key here
        }, {
            headers: {
                'Content-Type': 'application/json',
                'Authorization': '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',
            },
        });

        res.json({ reply: response.data.choices[0].text.trim() });
    } catch (error) {
        console.error('Error:', error);
        res.status(500).json({ error: 'Server error' });
    }
});

// Start the server
app.listen(PORT, () => {
    console.log(`Server running on port ${PORT}`);
});

// Function to send a message to the OpenAI Chat API
async function sendMessageToChatAPI(message) {
    const apiKey = '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'; // Replace with your OpenAI API key
    const endpoint = 'https://api.openai.com/v1/engines/davinci/completions'; // API endpoint

    const response = await fetch(endpoint, {
        method: 'POST',
        headers: {
            'Authorization': `Bearer ${apiKey}`,
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            model: 'davinci', // or another model depending on your preference
            prompt: message // user message to the AI
        })
    });

    return response.json();
}

// Function to handle user input and AI responses
async function handleChat() {
    // Get user input (e.g., from a text input field)
    const userInput = document.getElementById('userInput').value;

    // Send the user message to the OpenAI Chat API
    const response = await sendMessageToChatAPI(userInput);

    // Update the UI with the AI's response (e.g., display in a chat window)
    displayAIResponse(response.choices[0].text);

    // Clear the input field if needed
    document.getElementById('userInput').value = '';
}

// Function to display AI response in the chat window
function displayAIResponse(response) {
    // Update your UI to display the AI's response
    // For example, update a chat window with the response
}