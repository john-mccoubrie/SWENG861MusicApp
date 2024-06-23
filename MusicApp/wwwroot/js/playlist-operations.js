document.addEventListener('DOMContentLoaded', function () {
    // Attach event listeners to the playlist container
    const playlistContainer = document.getElementById('playlist-container');

    playlistContainer.addEventListener('click', function (event) {
        if (event.target.classList.contains('edit')) {
            const playlistItemId = event.target.getAttribute('data-playlist-item-id');
            const url = event.target.getAttribute('data-url');
            editSong(event.target, playlistItemId, url);
        } else if (event.target.classList.contains('delete')) {
            const playlistId = event.target.getAttribute('data-playlist-id');
            const songId = event.target.getAttribute('data-song-id');
            const url = event.target.getAttribute('data-url');
            deleteSong(event.target, playlistId, songId, url);
        }
    });
});

function editSong(button, playlistItemId, url) {
    const listItem = document.getElementById(`song-${playlistItemId}`);
    if (!listItem) {
        console.error("No list item found with ID:", `song-${playlistItemId}`);
        return;
    }

    // Hide existing buttons
    const editButton = listItem.querySelector('button.edit');
    const deleteButton = listItem.querySelector('button.delete');
    editButton.style.display = 'none';
    deleteButton.style.display = 'none';

    // Create and append the save button
    const saveButton = document.createElement('button');
    saveButton.textContent = 'Save';
    saveButton.classList.add('save');
    saveButton.onclick = function () {
        saveChanges(url, playlistItemId, listItem, saveButton);
    };
    listItem.appendChild(saveButton);

    // Convert all editable fields into input fields
    const fields = listItem.querySelectorAll('.editable');
    fields.forEach(field => {
        const input = document.createElement('input');
        input.type = 'text';
        input.value = field.innerText;
        field.innerText = '';
        field.appendChild(input);
        input.focus();
    });
}

function saveChanges(url, playlistItemId, listItem, saveButton) {
    const fields = listItem.querySelectorAll('.editable input');
    fields.forEach(input => {
        const field = input.parentElement.getAttribute('data-field');
        const value = input.value;

        $.ajax({
            url: url,
            type: 'POST',
            data: { playlistItemId: playlistItemId, field: field, value: value },
            success: function (response) {
                if (response.success) {
                    input.parentElement.innerText = value;
                    console.log('Changes saved successfully for field:', field);
                } else {
                    alert('Failed to save changes for field:', field);
                }
            },
            error: function () {
                alert('Error processing your request for field:', field);
            }
        });
    });

    // Restore original buttons and remove the save button
    saveButton.remove();
    listItem.querySelector('button.edit').style.display = '';
    listItem.querySelector('button.delete').style.display = '';
}

function deleteSong(button, playlistId, songId, url) {
    $.ajax({
        url: url,
        type: 'POST',
        data: { playlistId: playlistId, songId: songId },
        success: function (response) {
            if (response.success) {
                alert("Song removed successfully.");
                location.reload(); // Refreshes the page to reflect changes
            } else {
                alert("Failed to remove song: " + response.message);
            }
        },
        error: function () {
            alert("Error processing your request.");
        }
    });
}
