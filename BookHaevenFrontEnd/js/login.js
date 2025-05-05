document.addEventListener("DOMContentLoaded", () => {
  const form = document.getElementById("loginForm");

  form.addEventListener("submit", async (e) => {
    e.preventDefault();

    const formData = new FormData(form);
    const data = {
      username: formData.get("username"),
      password: formData.get("password")
    };

    try {
      const res = await fetch("http://localhost:5036/api/auth/login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data)
      });

      if (!res.ok) {
        const error = await res.json();
        document.getElementById("error").textContent = error.message || "Invalid credentials.";
        return;
      }

      const result = await res.json();
      localStorage.setItem("token", result.token);
      window.location.href = "index.html";
    } catch (err) {
      document.getElementById("error").textContent = "Something went wrong.";
    }
  });
});

