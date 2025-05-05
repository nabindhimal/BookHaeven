document.addEventListener("DOMContentLoaded", () => {
  let currentPage = 1;
  const pageSize = 9;

  const listContainer = document.getElementById("book-list");
  const paginationContainer = document.getElementById("pagination");

  async function loadBooks(page = 1) {
    try {
      const res = await fetch(`http://localhost:5036/api/books/paginated?page=${page}&pageSize=${pageSize}`);
      if (!res.ok) throw new Error("Failed to load books");

      const books = await res.json();
      listContainer.innerHTML = books.map(book => `
        <div class="bg-white rounded shadow p-4">
          <h2 class="text-xl font-semibold">${book.name}</h2>
          <p class="text-gray-600 mb-2">${book.author}</p>
          <p class="text-blue-500 font-bold mb-2">$${book.price}</p>
          <a href="/book?id=${book.id}" class="text-indigo-600 hover:underline">View Details</a>
        </div>
      `).join("");

      renderPagination(page);
    } catch (err) {
      listContainer.innerHTML = `<p class="text-red-500">${err.message}</p>`;
    }
  }

  function renderPagination(current) {
    const totalPages = 10; // Change this as needed (or fetch from backend if your API provides total pages)

    paginationContainer.innerHTML = "";

    for (let i = 1; i <= totalPages; i++) {
      const button = document.createElement("button");
      button.textContent = i;
      button.className = `px-3 py-1 mx-1 rounded ${i === current ? 'bg-indigo-600 text-white' : 'bg-gray-200 hover:bg-gray-300'}`;
      button.addEventListener("click", () => {
        currentPage = i;
        loadBooks(currentPage);
      });
      paginationContainer.appendChild(button);
    }
  }

  loadBooks(currentPage);
});

