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
      const token = result.token;
      
      localStorage.setItem("token", token);
      
      // Decode JWT payload
    const payloadBase64 = token.split('.')[1];
    const decodedPayload = JSON.parse(atob(payloadBase64));

    // Extract username and role from claims
    const username = decodedPayload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"];
    const role = decodedPayload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

    localStorage.setItem("username", username);
    localStorage.setItem("role", role);
      
      //const payload = parseJwt(token);
      //const role = payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
      
      if (role === "Admin") {
        window.location.href = "admin.html";
      } else {
        window.location.href = "index.html";
      }
      
      // window.location.href = "index.html";
    } catch (err) {
      document.getElementById("error").textContent = "Something went wrong.";
    }
  });
});


      
  
