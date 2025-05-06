document.addEventListener("DOMContentLoaded", () => {
  let currentPage = 1;
  const pageSize = 6;
  let token = localStorage.getItem("token");

  const listContainer = document.getElementById("book-list");
  const paginationContainer = document.getElementById("pagination");

  // Function to check authentication status
  function isAuthenticated() {
    token = localStorage.getItem("token");
    return token !== null;
  }

  async function loadBooks(page = 1) {
    try {
      listContainer.innerHTML = '<div class="col-span-3 text-center py-12"><div class="inline-block animate-spin rounded-full h-8 w-8 border-t-2 border-b-2 border-blue-500"></div><p class="mt-2 text-gray-600">Loading books...</p></div>';
      
      // Include authorization header if authenticated
      const headers = {};
      if (isAuthenticated()) {
        headers['Authorization'] = `Bearer ${token}`;
      }

      const res = await fetch(`http://localhost:5036/api/books/paginated?page=${page}&pageSize=${pageSize}`, {
        headers
      });
      
      if (!res.ok) throw new Error("Failed to load books");

      const books = await res.json();
      
      if (books.length === 0) {
        listContainer.innerHTML = '<div class="col-span-3 text-center py-12"><p class="text-gray-500">No books found</p></div>';
        return;
      }

      listContainer.innerHTML = books.map(book => `
        <div class="book-card bg-white rounded-lg shadow-md overflow-hidden hover:shadow-lg transition-shadow">
          ${isAuthenticated() ? `
            <button class="bookmark-btn" data-book-id="${book.id}" data-bookmarked="${book.isBookmarked}">
              <svg class="bookmark-icon" viewBox="0 0 24 24" fill="${book.isBookmarked ? 'gold' : 'none'}" stroke="currentColor" stroke-width="2">
                <path stroke-linecap="round" stroke-linejoin="round" d="M11.049 2.927c.3-.921 1.603-.921 1.902 0l1.519 4.674a1 1 0 00.95.69h4.915c.969 0 1.371 1.24.588 1.81l-3.976 2.888a1 1 0 00-.363 1.118l1.518 4.674c.3.922-.755 1.688-1.538 1.118l-3.976-2.888a1 1 0 00-1.176 0l-3.976 2.888c-.783.57-1.838-.197-1.538-1.118l1.518-4.674a1 1 0 00-.363-1.118l-3.976-2.888c-.784-.57-.38-1.81.588-1.81h4.914a1 1 0 00.951-.69l1.519-4.674z" />
              </svg>
            </button>
          ` : ''}
          <div class="h-48 bg-gray-200 overflow-hidden">
            <img src="http://localhost:5036${book.imageUrl}" alt="${book.name}" class="w-full h-full object-cover">
          </div>
          <div class="p-4">
            <h3 class="font-bold text-lg mb-1 truncate">${book.name}</h3>
            <p class="text-gray-600 mb-2">${book.author}</p>
            <div class="flex justify-between items-center">
              ${book.isOnSale && book.discountedPrice 
                ? `<span class="line-through text-red-500 mr-2">$${book.price.toFixed(2)}</span>
                   <span class="text-green-600 font-semibold">$${book.discountedPrice.toFixed(2)}</span>`
                : `<span class="text-black font-semibold">$${book.price.toFixed(2)}</span>`
              }
              <div class="flex items-center">
                <span class="text-yellow-500">★</span>
                <span class="ml-1 text-gray-700">${(book.averageRating || 0).toFixed(1)}</span>
              </div>
            </div>
		            


<a href="book.html?id=${book.id}" class="mt-2 block text-center bg-blue-500 text-white py-2 rounded hover:bg-blue-600 transition">
  View Details
</a>
<button onclick="addToCart('${book.id}')" class="mt-2 w-full bg-green-500 text-white py-2 rounded hover:bg-green-600 transition">
  Add to Cart
</button>


          </div>
        </div>
      `).join("");

      // Add event listeners to bookmark buttons
      if (isAuthenticated()) {
        document.querySelectorAll('.bookmark-btn').forEach(btn => {
          btn.addEventListener('click', toggleBookmark);
        });
      }

      renderPagination(page);
    } catch (error) {
      console.error('Error loading books:', error);
      listContainer.innerHTML = `
        <div class="col-span-3 text-center py-12">
          <p class="text-red-500 mb-4">Error loading books. Please try again.</p>
          <button onclick="window.location.reload()" class="px-4 py-2 bg-gray-200 rounded hover:bg-gray-300">
            Retry
          </button>
        </div>
      `;
    }
  }

  async function toggleBookmark(e) {
    const btn = e.currentTarget;
    const bookId = btn.getAttribute('data-book-id');
    const isBookmarked = btn.getAttribute('data-bookmarked') === 'true';
    const svg = btn.querySelector('svg');
    
    try {
      if (!isAuthenticated()) {
        throw new Error('Please log in to bookmark books');
      }

      // Optimistic UI update
      const newBookmarkedState = !isBookmarked;
      svg.setAttribute('fill', newBookmarkedState ? 'gold' : 'none');
      btn.setAttribute('data-bookmarked', newBookmarkedState.toString());

      if (newBookmarkedState) {
        // Adding a new bookmark
        const response = await fetch('http://localhost:5036/api/bookmarks', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
          },
          body: JSON.stringify({ bookId })
        });

        if (!response.ok) {
          throw new Error('Failed to add bookmark');
        }
      } else {
        // Removing a bookmark - we need to find the bookmark ID first
        const bookmarksResponse = await fetch('http://localhost:5036/api/bookmarks', {
          headers: {
            'Authorization': `Bearer ${token}`
          }
        });

        if (!bookmarksResponse.ok) {
          throw new Error('Failed to fetch bookmarks');
        }

        const bookmarks = await bookmarksResponse.json();
        const bookmarkToDelete = bookmarks.find(b => b.bookId === bookId);

        if (!bookmarkToDelete) {
          throw new Error('Bookmark not found');
        }

        // Now delete using the bookmark ID
        const deleteResponse = await fetch(`http://localhost:5036/api/bookmarks/${bookmarkToDelete.id}`, {
          method: 'DELETE',
          headers: {
            'Authorization': `Bearer ${token}`
          }
        });

        if (!deleteResponse.ok) {
          throw new Error('Failed to remove bookmark');
        }
      }
    } catch (error) {
      console.error('Error toggling bookmark:', error);
      // Revert UI on error
      svg.setAttribute('fill', isBookmarked ? 'gold' : 'none');
      btn.setAttribute('data-bookmarked', isBookmarked.toString());
      alert(error.message);
    }
  }

  function renderPagination(current) {
    const totalPages = 10; // Adjust based on your API

    paginationContainer.innerHTML = "";

    // Previous button
    if (current > 1) {
      const prevButton = document.createElement("button");
      prevButton.innerHTML = '&laquo;';
      prevButton.className = 'px-3 py-1 mx-1 rounded bg-gray-200 hover:bg-gray-300';
      prevButton.addEventListener("click", () => {
        currentPage = current - 1;
        loadBooks(currentPage);
      });
      paginationContainer.appendChild(prevButton);
    }

    // Page numbers
    const startPage = Math.max(1, current - 2);
    const endPage = Math.min(totalPages, current + 2);
    
    for (let i = startPage; i <= endPage; i++) {
      const button = document.createElement("button");
      button.textContent = i;
      button.className = `px-3 py-1 mx-1 rounded ${i === current ? 'bg-blue-500 text-white' : 'bg-gray-200 hover:bg-gray-300'}`;
      button.addEventListener("click", () => {
        currentPage = i;
        loadBooks(currentPage);
      });
      paginationContainer.appendChild(button);
    }

    // Next button
    if (current < totalPages) {
      const nextButton = document.createElement("button");
      nextButton.innerHTML = '&raquo;';
      nextButton.className = 'px-3 py-1 mx-1 rounded bg-gray-200 hover:bg-gray-300';
      nextButton.addEventListener("click", () => {
        currentPage = current + 1;
        loadBooks(currentPage);
      });
      paginationContainer.appendChild(nextButton);
    }
  }

  // Initial load
  loadBooks(currentPage);

  // Listen for storage changes (login/logout)
  window.addEventListener('storage', (event) => {
    if (event.key === 'token') {
      loadBooks(currentPage);
    }
  });

  // Listen for auth changes from our own tab
  window.addEventListener('authChange', () => {
    loadBooks(currentPage);
  });
});















// Add this near the top with other functions
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
    
    // Show success message
    showToast('Item added to cart successfully');
    
    // Update cart count in UI
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

























