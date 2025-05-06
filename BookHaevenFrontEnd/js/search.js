document.addEventListener("DOMContentLoaded", () => {
  // Create the search section
  const searchSection = document.createElement("section");
  searchSection.className = "max-w-7xl mx-auto px-4 py-8";

  // Create a container for the header and search form
  const headerContainer = document.createElement("div");
  headerContainer.className = "flex items-center justify-between mb-8";

  const logoLink = document.createElement("a");
  logoLink.href = "index.html";
  logoLink.className = "text-2xl font-bold text-blue-600 hover:text-blue-800 transition-colors";
  logoLink.textContent = "Book Haven";
  headerContainer.appendChild(logoLink);

  // Search form HTML
  const searchForm = document.createElement("form");
  searchForm.id = "searchForm";
  searchForm.className = "flex flex-wrap gap-4 items-center";
  searchForm.innerHTML = `
    <input 
      type="text" 
      name="title" 
      id="searchTitle" 
      class="px-4 py-2 rounded border border-gray-300 flex-1 min-w-[200px] focus:outline-none focus:ring-2 focus:ring-blue-500"
      placeholder="Search books"
      aria-label="Search books"
    />
    <select 
      id="genreFilter" 
      name="genre" 
      class="px-4 py-2 rounded border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-500"
      aria-label="Filter by genre"
    >
      <option value="">All Genres</option>
      <option value="fiction">Fiction</option>
      <option value="non-fiction">Non-fiction</option>
      <option value="romance">Romance</option>
      <option value="mystery">Mystery</option>
      <option value="sci-fi">Science Fiction</option>
    </select>
    <select 
      id="sortBy" 
      name="sortBy" 
      class="px-4 py-2 rounded border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-500"
      aria-label="Sort by"
    >
      <option value="">Sort By</option>
      <option value="title">Title (A-Z)</option>
      <option value="title_desc">Title (Z-A)</option>
      <option value="price">Price (Low to High)</option>
      <option value="price_desc">Price (High to Low)</option>
      <option value="rating">Rating</option>
    </select>
    <button 
      type="submit" 
      class="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600 transition-colors"
      aria-label="Search"
    >
      Search
    </button>
  `;
  headerContainer.appendChild(searchForm);

  // Results container
  const resultsContainer = document.createElement("div");
  resultsContainer.id = "searchResultsContainer";
  resultsContainer.className = "grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6";

  // Add everything to the search section
  searchSection.appendChild(headerContainer);
  searchSection.appendChild(resultsContainer);

  document.body.appendChild(searchSection);

  // Add event listener to form
  searchForm.addEventListener("submit", handleSearchSubmit);
});

function handleSearchSubmit(e) {
  e.preventDefault();

  const title = document.getElementById("searchTitle").value.trim();
  const genre = document.getElementById("genreFilter").value;
  const sortBy = document.getElementById("sortBy").value;

  console.log("Search parameters:", { title, genre, sortBy });

  const resultsContainer = document.getElementById("searchResultsContainer");
  resultsContainer.innerHTML = `
    <div class="col-span-full text-center py-12">
      <div class="inline-block animate-spin rounded-full h-8 w-8 border-t-2 border-b-2 border-blue-500"></div>
      <p class="mt-2 text-gray-600">Searching for books...</p>
    </div>
  `;

  // Build query parameters with correct case (Title, Genre, SortBy)
  const apiParams = new URLSearchParams();
  if (title) apiParams.append('Title', title);
  if (genre) apiParams.append('Genre', genre);
  if (sortBy) apiParams.append('SortBy', sortBy);

  const apiUrl = `http://localhost:5036/api/books/search?${apiParams.toString()}`;
  console.log("API URL:", apiUrl);

  fetch(apiUrl, {
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token') || ''}`
    }
  })
    .then(response => {
      console.log("Response status:", response.status);
      if (!response.ok) {
        return response.text().then(text => {
          throw new Error(`HTTP error! status: ${response.status}, body: ${text}`);
        });
      }
      return response.json();
    })
    .then(data => {
      console.log("API response data:", data);
      displaySearchResults(data);
    })
    .catch(err => {
      console.error("Error:", err);
      displaySearchError(err.message);
    });
}

function displaySearchResults(data) {
  const resultsContainer = document.getElementById("searchResultsContainer");
  resultsContainer.innerHTML = '';

  if (data.message || !Array.isArray(data) || data.length === 0) {
    resultsContainer.innerHTML = `
      <div class="col-span-full text-center py-12">
        <p class="text-xl text-gray-500">${data.message || 'No books found matching your search criteria.'}</p>
        <button onclick="document.getElementById('searchForm').submit()" 
                class="mt-4 px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600">
          Try Again
        </button>
      </div>
    `;
    return;
  }

  data.forEach(book => {
    const bookCard = document.createElement("div");
    bookCard.className = "bg-white rounded-lg overflow-hidden shadow-md hover:shadow-lg transition-shadow";

    // Handle both PascalCase and camelCase field names
    const bookTitle = book.Name || book.name || 'Untitled';
    const bookAuthor = book.Author || book.author || 'Unknown Author';
    const bookPrice = book.Price || book.price || 0;
    const bookRating = book.Rating || book.rating || book.averageRating || 0;
    const bookImage = book.CoverImage || book.imageUrl || 'https://via.placeholder.com/300x200?text=No+Cover';
    const bookId = book.Id || book.id;

    bookCard.innerHTML = `
      <div class="h-48 bg-gray-200 overflow-hidden">
        <img src="${bookImage.startsWith('http') ? bookImage : `http://localhost:5036${bookImage}`}" 
             alt="${bookTitle}" 
             class="w-full h-full object-cover">
      </div>
      <div class="p-4">
        <h3 class="font-bold text-lg mb-1 truncate">${bookTitle}</h3>
        <p class="text-gray-600 mb-2">${bookAuthor}</p>
        <div class="flex justify-between items-center">
          ${book.isOnSale && book.discountedPrice 
            ? `<span class="line-through text-red-500 mr-2">$${bookPrice.toFixed(2)}</span>
               <span class="text-green-600 font-semibold">$${book.discountedPrice.toFixed(2)} (${book.discountPercentage}% OFF)</span>`
            : `<span class="text-black font-semibold">$${bookPrice.toFixed(2)}</span>`
          }
          <div class="flex items-center">
            <span class="text-yellow-500">★</span>
            <span class="ml-1 text-gray-700">${bookRating.toFixed(1)}</span>
          </div>
        </div>
       



<a href="/book?id=${bookId}" 
   class="mt-2 block text-center bg-blue-500 text-white py-2 rounded hover:bg-blue-600 transition">
  View Details
</a>
<button onclick="addToCart('${bookId}')" 
        class="mt-2 w-full bg-green-500 text-white py-2 rounded hover:bg-green-600 transition">
  Add to Cart
</button>
      </div>
    `;

    resultsContainer.appendChild(bookCard);
  });
}

function displaySearchError(message) {
  const resultsContainer = document.getElementById("searchResultsContainer");
  resultsContainer.innerHTML = `
    <div class="col-span-full text-center py-12">
      <p class="text-red-500 mb-4">${message || 'Error loading search results'}</p>
      <button onclick="document.getElementById('searchForm').submit()" 
              class="px-4 py-2 bg-gray-200 rounded hover:bg-gray-300">
        Try Again
      </button>
    </div>
  `;
}



async function addToCart(bookId, quantity = 1) {
  try {
    const token = localStorage.getItem("token");
    if (!token) {
      throw new Error('Please log in to add items to cart');
    }

    const response = await fetch('http://localhost:5036/api/cart/add', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      },
      body: JSON.stringify({ bookId, quantity })
    });

    if (!response.ok) {
      const errorData = await response.json();
      throw new Error(errorData.message || 'Failed to add to cart');
    }

    const result = await response.json();
    
    showToast('Item added to cart successfully');
    updateCartCount();
    
    return result;
  } catch (error) {
    console.error('Error adding to cart:', error);
    showToast(error.message, 'error');
  }
}

function showToast(message, type = 'success') {
  const toast = document.createElement('div');
  toast.className = `fixed bottom-4 right-4 px-4 py-2 rounded shadow-lg ${
    type === 'success' ? 'bg-green-500' : 'bg-red-500'
  } text-white`;
  toast.textContent = message;
  document.body.appendChild(toast);
  
  setTimeout(() => {
    toast.remove();
  }, 3000);
}

async function updateCartCount() {
  const token = localStorage.getItem("token");
  if (!token) return;

  try {
    const response = await fetch('http://localhost:5036/api/cart/count', {
      headers: {
        'Authorization': `Bearer ${token}`
      }
    });

    if (response.ok) {
      const count = await response.json();
      const cartCountElement = document.getElementById('cart-count');
      if (cartCountElement) {
        cartCountElement.textContent = count;
        cartCountElement.classList.toggle('hidden', count === 0);
      }
    }
  } catch (error) {
    console.error('Error updating cart count:', error);
  }
}


