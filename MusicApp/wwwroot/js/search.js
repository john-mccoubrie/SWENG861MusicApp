document.addEventListener('DOMContentLoaded', function () {
    const searchForm = document.getElementById('searchForm');
    searchForm.addEventListener('submit', function (event) {
        event.preventDefault();
        const query = document.getElementById('searchQuery').value;
        $.ajax({
            url: '/Home/Search', // Confirm this URL with your actual routing
            type: 'POST',
            data: { searchQuery: query },
            success: function (data) {
                $('#searchResultsContainer').html(data); // Update the container with the returned HTML
            },
            error: function (error) {
                console.error('Error loading search results:', error.statusText);
            }
        });
    });
});



function loadSearchResults(query) {
    $.ajax({
        url: '/Home/Search',
        type: 'POST',
        data: { searchQuery: query },
        success: function (data) {
            $('#searchResultsContainer').html(data);
        },
        error: function () {
            console.error('Error loading search results.');
        }
    });
}
