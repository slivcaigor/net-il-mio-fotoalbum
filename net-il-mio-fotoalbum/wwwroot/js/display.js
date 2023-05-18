axios.get('/api/PhotosApi/GetPhotos')
    .then(function (response) {
        var data = response.data;
        var photos = data.photos;
        var searchInput = document.getElementById('search-input');
        var photoContainer = document.getElementById('photo-container');
        var currentPage = data.page;
        var totalPages = data.totalPages;
        var pageSize = data.pageSize; // Add this line
        var previousButton = document.getElementById('previous-button');
        var nextButton = document.getElementById('next-button');

        function displayPhotos(photos) {
            photoContainer.innerHTML = '';

            if (photos.length === 0) {
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
            currentPage = 1;

            axios.get(`/api/PhotosApi/GetPhotos?search=${searchValue}&page=${currentPage}&pageSize=${pageSize}`)
                .then(function (response) {
                    var data = response.data;
                    var pagePhotos = data.photos;
                    totalPages = data.totalPages;
                    displayPhotos(pagePhotos);
                })
                .catch(function (error) {
                    console.log(error);
                });
        });

        displayPhotos(photos);

        function goToPage(page) {
            if (page < 1 || page > totalPages) {
                return;
            }

            currentPage = page;

            axios.get(`/api/PhotosApi/GetPhotos?search=&page=${currentPage}&pageSize=${pageSize}`)
                .then(function (response) {
                    var data = response.data;
                    var pagePhotos = data.photos;
                    displayPhotos(pagePhotos);
                })
                .catch(function (error) {
                    console.log(error);
                });
        }

        previousButton.addEventListener('click', function () {
            goToPage(currentPage - 1);
        });

        nextButton.addEventListener('click', function () {
            goToPage(currentPage + 1);
        });
    })
    .catch(function (error) {
        console.log(error);
    });
