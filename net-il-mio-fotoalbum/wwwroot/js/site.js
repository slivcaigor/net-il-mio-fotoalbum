document.getElementById('contactForm').addEventListener('submit', function (event) {
    // Prevent the default submit behavior
    event.preventDefault();

    // Get the data from the form
    var name = document.getElementById('name').value;
    var email = document.getElementById('email').value;
    var message = document.getElementById('text').value;

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
                text: "Message sent successfully",
                duration: 5000
            })
            myToast.showToast();

            // Clear the form
            document.getElementById('contactForm').reset();
        })
        .catch(function (error) {
            console.log('An error occurred: ', error);

            var myToast = Toastify({
                text: "An error occurred while sending the message",
                duration: 5000
            })
            myToast.showToast();
        });
});
