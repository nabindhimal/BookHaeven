// announcement.js
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

    // Display announcements (you can customize this part)
    displayAnnouncements(announcements);
  } catch (error) {
    console.error("Error fetching announcements:", error);
    // You might want to show an error message to the user
  }
});

function displayAnnouncements(announcements) {
  // Create a container for announcements at the top of the page
  const announcementsContainer = document.createElement("div");
  announcementsContainer.className = "max-w-5xl mx-auto mb-8";

  // Add a heading
  const heading = document.createElement("h2");
  heading.className = "text-2xl font-bold mb-4 text-center";
  heading.textContent = "Announcements";
  announcementsContainer.appendChild(heading);

  // Create a list for announcements
  const announcementsList = document.createElement("div");
  announcementsList.className = "bg-white rounded-lg shadow-md p-6 space-y-4";

  if (announcements.length === 0) {
    const emptyMessage = document.createElement("p");
    emptyMessage.className = "text-gray-500 text-center";
    emptyMessage.textContent = "No active announcements at this time.";
    announcementsList.appendChild(emptyMessage);
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
      
      announcementsList.appendChild(announcementItem);
    });
  }

  announcementsContainer.appendChild(announcementsList);
  
  // Insert announcements at the top of the page
  const mainContent = document.querySelector("body > div.max-w-5xl");
  if (mainContent) {
    mainContent.parentNode.insertBefore(announcementsContainer, mainContent);
  }
}
