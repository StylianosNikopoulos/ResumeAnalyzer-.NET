async function submitKeywords() {
    const input = document.getElementById("keywordInput").value.trim();
    if (!input) {
        alert("Enter keywords.");
        return;
    }

    const keywords = input.split(",").map(k => k.trim()).filter(k => k);
    if (keywords.length === 0) {
        alert("Enter valid keywords.");
        return;
    }

    try {
        const response = await fetch("https://localhost:7083/api/resumes/filter", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                // No Authorization header here
            },
            body: JSON.stringify({ keywords: keywords })
        });

        if (response.ok) {
            const resumes = await response.json();
            console.log("Filtered Resumes:", resumes);
            alert("Filter successful. Check console for results.");
            // Optionally, call a function to display the resumes in your UI
        } else {
            const error = await response.json();
            alert(`Failed to filter: ${error.message || "Unknown error"}`);
        }
    } catch (err) {
        console.error("Error:", err);
        alert("An unexpected error occurred.");
    }
}
