fetch("scripts/navbar/navbar.html")
    .then(response => response.text())
    .then(data => {
        // document.getElementById("navbar").innerHTML = data; // legacy.
        const header = document.querySelector("header");
        header.insertAdjacentHTML('afterend', data);
    })
    .catch(error => console.error(error));