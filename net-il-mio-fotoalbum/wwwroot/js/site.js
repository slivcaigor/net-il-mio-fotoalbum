document.getElementById('contactForm').addEventListener('submit', function (event) {
    // Prevent the default submit behavior
    event.preventDefault();

    // Get the data from the form
    var name = document.getElementById('name').value;
    var email = document.getElementById('email').value;
    var message = document.getElementById('text').value;

    // Validation for name, email and message
    if (name === "" || email === "" || message === "") {
        var myToast = Toastify({
            text: "All fields are required!",
            className: "errorToast",
            duration: 5000
        })
        myToast.showToast();
        return;
    }

    if (name.length > 100) {
        var myToast = Toastify({
            text: "Name can't be longer than 100 characters!",
            background: "linear-gradient(to right, #ff5f6d, #ffc371)",
            className: "errorToast",
            duration: 5000
        })
        myToast.showToast();
        return;
    }

    if (!/^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$/.test(email)) {
        var myToast = Toastify({
            text: "Invalid email format!",
            className: "errorToast",
            duration: 5000
        })
        myToast.showToast();
        return;
    }

    if (message.length > 500) {
        var myToast = Toastify({
            text: "Message can't be longer than 500 characters!",
            className: "errorToast",
            duration: 5000
        })
        myToast.showToast();
        return;
    }

    // Create an object with the data
    var data = {
        name: name,
        email: email,
        text: message
    };

    axios.post('/api/Contact', data)
        .then(function (response) {
            console.log('Data saved successfully: ', response);

            var myToast = Toastify({
                text: "Message sent successfully!",
                className: "successToast",
                duration: 5000
            })
            myToast.showToast();

            // Clear the form
            document.getElementById('contactForm').reset();
        })
        .catch(function (error) {
            console.log('An error occurred: ', error);

            var myToast = Toastify({
                text: "An error occurred while sending the message.",
                className: "errorToast",
                duration: 5000
            })
            myToast.showToast();
        });
});
