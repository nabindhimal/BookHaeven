

const express = require('express');
const path = require('path');
const app = express();
const port = 3000;

// Serve static files from current directory
app.use(express.static(path.join(__dirname)));

// Route for book details - must come AFTER static files middleware
app.get('/book', (req, res) => {
  res.sendFile(path.join(__dirname, 'book.html'));
});

// Start server
app.listen(port, () => {
  console.log(`Server running at http://localhost:${port}`);
});
