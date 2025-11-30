let connection = null;

function initSignalR() {
    if (typeof signalR === 'undefined') {
        console.error("SignalR is not loaded. Make sure the SignalR script is included.");
        return;
    }

    const signalRUrl = window.location.hostname === 'localhost' 
        ? "http://localhost:5113/stationHub" 
        : `${window.location.protocol}//${window.location.host}/stationHub`;
    
    connection = new signalR.HubConnectionBuilder()
        .withUrl(signalRUrl)
        .withAutomaticReconnect()
        .build();

    connection.start().then(() => {
        console.log("SignalR Connected");
    }).catch(err => {
        console.error("SignalR Connection Error:", err);
    });

    connection.on("StationUpdated", (stationJson) => {
        console.log("Station Updated:", stationJson);
        try {
            const station = typeof stationJson === 'string' ? JSON.parse(stationJson) : stationJson;
            showNotification(`Station ${station.stationName || station.StationName} has been updated`, "info");
            updateStationRow(station);
        } catch (e) {
            console.error("Error parsing StationUpdated:", e);
            refreshStationList();
        }
    });

    connection.on("StationCreated", (stationJson) => {
        console.log("Station Created:", stationJson);
        try {
            const station = typeof stationJson === 'string' ? JSON.parse(stationJson) : stationJson;
            showNotification(`New station ${station.stationName || station.StationName} has been created`, "success");
            refreshStationList();
        } catch (e) {
            console.error("Error parsing StationCreated:", e);
            refreshStationList();
        }
    });

    connection.on("StationDeleted", (stationId) => {
        console.log("Station Deleted:", stationId);
        showNotification(`Station ${stationId} has been deleted`, "warning");
        removeStationRow(stationId);
    });

    connection.on("StationActivated", (stationJson) => {
        console.log("Station Activated:", stationJson);
        try {
            const station = typeof stationJson === 'string' ? JSON.parse(stationJson) : stationJson;
            showNotification(`Station ${station.stationName || station.StationName} has been activated`, "success");
            updateStationRow(station);
        } catch (e) {
            console.error("Error parsing StationActivated:", e);
            refreshStationList();
        }
    });

    connection.on("StationAvailabilityUpdated", (dataJson) => {
        console.log("Station Availability Updated:", dataJson);
        try {
            const data = typeof dataJson === 'string' ? JSON.parse(dataJson) : dataJson;
            const stationId = data.stationId || data.StationId;
            const currentAvailable = data.currentAvailable || data.CurrentAvailable;
            updateStationAvailability(stationId, currentAvailable);
        } catch (e) {
            console.error("Error parsing StationAvailabilityUpdated:", e);
        }
    });

    connection.onreconnecting(() => {
        console.log("SignalR Reconnecting...");
    });

    connection.onreconnected(() => {
        console.log("SignalR Reconnected");
    });

    connection.onclose(() => {
        console.log("SignalR Connection Closed");
    });
}

function showNotification(message, type) {
    const alertClass = type === "success" ? "alert-success" : 
                      type === "warning" ? "alert-warning" : "alert-info";
    const notification = document.createElement("div");
    notification.className = `alert ${alertClass} alert-dismissible fade show position-fixed top-0 end-0 m-3`;
    notification.style.zIndex = "9999";
    notification.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;
    document.body.appendChild(notification);
    setTimeout(() => {
        notification.remove();
    }, 5000);
}

function refreshStationList() {
    const urlParams = new URLSearchParams(window.location.search);
    const name = urlParams.get('name') || '';
    const location = urlParams.get('location') || '';
    const isActive = urlParams.get('isActive') || '';
    const pageNumber = urlParams.get('pageNumber') || '1';
    
    const newUrl = new URL(window.location.href);
    newUrl.searchParams.set('name', name);
    newUrl.searchParams.set('location', location);
    if (isActive) newUrl.searchParams.set('isActive', isActive);
    newUrl.searchParams.set('pageNumber', pageNumber);
    
    window.location.href = newUrl.toString();
}

function updateStationRow(station) {
    const stationId = station.stationId || station.StationId;
    if (!stationId) {
        console.error("Station ID not found in update data:", station);
        refreshStationList();
        return;
    }
    
    const row = document.querySelector(`tr[data-station-id="${stationId}"]`);
    if (row) {
        const stationName = station.stationName || station.StationName || "";
        const stationCode = station.stationCode || station.StationCode || "";
        const address = station.address || station.Address || "";
        const city = station.city || station.City || "";
        const capacity = station.capacity || station.Capacity || 0;
        const currentAvailable = station.currentAvailable || station.CurrentAvailable || 0;
        const isActive = station.isActive !== undefined ? station.isActive : (station.IsActive !== undefined ? station.IsActive : true);

        const nameCell = row.querySelector(".station-name");
        if (nameCell) nameCell.textContent = stationName;
        
        const codeCell = row.querySelector("td:nth-child(2)");
        if (codeCell) codeCell.textContent = stationCode;
        
        const addressCell = row.querySelector("td:nth-child(4)");
        if (addressCell) addressCell.textContent = address;
        
        const cityCell = row.querySelector("td:nth-child(5)");
        if (cityCell) cityCell.textContent = city;
        
        const capacityCell = row.querySelector("td:nth-child(6)");
        if (capacityCell) capacityCell.textContent = capacity;
        
        const availableCell = row.querySelector(".current-available");
        if (availableCell) availableCell.textContent = currentAvailable;
        
        const statusCell = row.querySelector(".station-status");
        if (statusCell) {
            statusCell.innerHTML = isActive 
                ? '<span class="badge bg-success">Active</span>'
                : '<span class="badge bg-secondary">Inactive</span>';
        }
        
        console.log(`Updated station row ${stationId} in table`);
    } else {
        console.warn(`Station row ${stationId} not found, refreshing list`);
        refreshStationList();
    }
}

function removeStationRow(stationId) {
    const row = document.querySelector(`tr[data-station-id="${stationId}"]`);
    if (row) {
        row.remove();
    } else {
        refreshStationList();
    }
}

function updateStationAvailability(stationId, currentAvailable) {
    const row = document.querySelector(`tr[data-station-id="${stationId}"]`);
    if (row) {
        const availableCell = row.querySelector(".current-available");
        if (availableCell) {
            availableCell.textContent = currentAvailable;
        }
    }
}

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initSignalR);
} else {
    initSignalR();
}

