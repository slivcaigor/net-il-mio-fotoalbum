axios.get('/api/PhotosApi/GetPhotos')
    .then(function (response) {
        var photos = response.data;
        var searchInput = document.getElementById('search-input');
        var photoContainer = document.getElementById('photo-container');

        function displayPhotos(photos) {
            photoContainer.innerHTML = '';

            if (photos.length === 0) {
                var noPhotosMessage = document.createElement('p');
                noPhotosMessage.classList.add('mt-4');
                photoContainer.appendChild(noPhotosMessage);

                Toastify({
                    text: 'No photos found',
                    duration: 1500,
                    gravity: 'center',
                    position: 'center',
                    backgroundColor: 'red',
                    stopOnFocus: true
                }).showToast();
            } else {
                photos.forEach(function (photo) {
                    var photoElement = document.createElement('div');
                    photoElement.innerHTML = `
            <div class="bg-white border border-gray-100 transition transform duration-700 hover:shadow-xl hover:scale-105 p-4 rounded-lg relative">
              <img class="w-full mx-auto transform transition duration-300 hover:scale-105" src="${photo.image}" alt="${photo.title}" loading="lazy">
              <div class="flex flex-col items-center my-3 space-y-2">
                <h1 class="text-gray-900 poppins text-lg">${photo.title}</h1>
                <p class="text-gray-500 poppins text-sm text-center">${photo.description}</p>
              </div>
            </div>
          `;
                    photoContainer.appendChild(photoElement);
                });
            }
        }

        searchInput.addEventListener('input', function () {
            var searchValue = searchInput.value.trim().toLowerCase();

            var filteredPhotos = photos.filter(function (photo) {
                return photo.title.toLowerCase().includes(searchValue);
            });

            displayPhotos(filteredPhotos);
        });

        displayPhotos(photos);
    })
    .catch(function (error) {
        console.log(error);
    });
