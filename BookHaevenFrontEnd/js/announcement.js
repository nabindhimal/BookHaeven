document.addEventListener("DOMContentLoaded", async () => {
  try {
    // Check if user is logged in (has token)
    const token = localStorage.getItem("token");
    if (!token) {
      console.log("User not logged in - skipping announcements");
      return;
    }

    // Fetch active announcements
    const response = await fetch("http://localhost:5036/api/announcements/active", {
      headers: {
        'Authorization': `Bearer ${token}`
      }
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const announcements = await response.json();
    console.log("Active announcements:", announcements);

    // Display announcements with toggle
    displayAnnouncementsWithToggle(announcements);
  } catch (error) {
    console.error("Error fetching announcements:", error);
    // You might want to show an error message to the user
  }
});

function displayAnnouncementsWithToggle(announcements) {
  // Create main container
  const announcementsContainer = document.createElement("div");
  announcementsContainer.className = "max-w-5xl mx-auto mb-8";
  announcementsContainer.id = "announcements-container";

  // Create toggle button container
  const toggleContainer = document.createElement("div");
  toggleContainer.className = "flex justify-between items-center mb-2";

  // Add heading
  const heading = document.createElement("h2");
  heading.className = "text-2xl font-bold";
  heading.textContent = "Announcements";
  toggleContainer.appendChild(heading);

  // Add toggle button
  const toggleButton = document.createElement("button");
  toggleButton.className = "text-blue-600 hover:text-blue-800 focus:outline-none";
  toggleButton.innerHTML = `
    <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
    </svg>
  `;
  toggleButton.title = "Toggle announcements";
  toggleButton.addEventListener("click", toggleAnnouncements);
  toggleContainer.appendChild(toggleButton);

  announcementsContainer.appendChild(toggleContainer);

  // Create announcements content (initially visible)
  const announcementsContent = document.createElement("div");
  announcementsContent.className = "bg-white rounded-lg shadow-md p-6 space-y-4";
  announcementsContent.id = "announcements-content";

  if (announcements.length === 0) {
    const emptyMessage = document.createElement("p");
    emptyMessage.className = "text-gray-500 text-center";
    emptyMessage.textContent = "No active announcements at this time.";
    announcementsContent.appendChild(emptyMessage);
  } else {
    announcements.forEach(announcement => {
      const announcementItem = document.createElement("div");
      announcementItem.className = "border-b border-gray-200 pb-4 last:border-0 last:pb-0";
      
      announcementItem.innerHTML = `
        <h3 class="text-lg font-semibold text-blue-600">${announcement.message}</h3>
        <p class="text-gray-600 mt-1">${announcement.endTime}</p>
        <p class="text-sm text-gray-400 mt-2">
          Posted on: ${new Date(announcement.createdAt).toLocaleDateString()}
          ${announcement.expiresAt ? ` | Expires: ${new Date(announcement.expiresAt).toLocaleDateString()}` : ''}
        </p>
      `;
      
      announcementsContent.appendChild(announcementItem);
    });
  }

  announcementsContainer.appendChild(announcementsContent);
  
  // Insert announcements at the top of the page
  const mainContent = document.querySelector("body > div.max-w-5xl");
  if (mainContent) {
    mainContent.parentNode.insertBefore(announcementsContainer, mainContent);
  }

  // Add toggle functionality
  function toggleAnnouncements() {
    const content = document.getElementById("announcements-content");
    const isHidden = content.classList.contains("hidden");
    
    if (isHidden) {
      content.classList.remove("hidden");
      toggleButton.innerHTML = `
        <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
        </svg>
      `;
    } else {
      content.classList.add("hidden");
      toggleButton.innerHTML = `
        <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 15l7-7 7 7" />
        </svg>
      `;
    }
  }
}
