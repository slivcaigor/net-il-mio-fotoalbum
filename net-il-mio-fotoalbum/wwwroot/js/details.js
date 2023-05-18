document.addEventListener("DOMContentLoaded", function () {
    // Retrieve the photo ID from the URL
    const photoId = window.location.pathname.split('/').pop();

    // Make an API request to get the photo details
    axios.get(`/api/PhotosApi/GetPhoto/${photoId}`)
        .then(function (response) {
            const photo = response.data;

            // Update the DOM with the photo details
            document.getElementById('photo-image').src = photo.image;
            document.getElementById('photo-image').alt = photo.title;
            document.getElementById('title').textContent = photo.title;
            document.getElementById('description').textContent = photo.description;

            // Update the categories
            const categoriesList = document.getElementById('categories').querySelector('ul');
            if (categoriesList && Array.isArray(photo.categories)) {
                photo.categories.forEach(function (category) {
                    if (category && category.name) {
                        const listItem = document.createElement('li');
                        listItem.textContent = category.name;
                        categoriesList.appendChild(listItem);
                    }
                });
            }

        })
        .catch(function (error) {
            console.log(error);
        });
});
