# Postman gRPC Service Testing Guide

## Overview
This guide explains how to test the EVCS gRPC Service using Postman.

## Prerequisites
- Postman installed (version 10.0 or later with gRPC support)
- gRPC Service running at `http://localhost:5112`

## Setting Up Postman for gRPC

### 1. Create New gRPC Request
1. Open Postman
2. Click **New** → Select **gRPC Request**
3. Enter a name for your request (e.g., "Get All Stations")

### 2. Configure Server URL
1. In the gRPC request, enter the server URL: `http://localhost:5112`
2. Click **Use Server Reflection** if available, or manually import proto files

### 3. Import Proto Files
If server reflection is not available:
1. Click **Import** in Postman
2. Select the proto files:
   - `EVCS.GrpcService.TriNM/Protos/common.proto`
   - `EVCS.GrpcService.TriNM/Protos/Station.proto`
3. Postman will parse the proto definitions

## Available gRPC Methods

### Station Service: `StationGRPC`

#### 1. GetAllAsync
**Method:** `GetAllAsync`  
**Request Type:** `EmptyRequest`  
**Response Type:** `StationList`

**Request Body:**
```json
{}
```

**Example Response:**
```json
{
  "items": [
    {
      "stationId": 1,
      "stationCode": "ST001",
      "stationName": "Main Station",
      "address": "123 Main St",
      "city": "Ho Chi Minh",
      "province": "Ho Chi Minh",
      "latitude": 10.762622,
      "longitude": 106.660172,
      "capacity": 10,
      "currentAvailable": 8,
      "owner": "EVCS Company",
      "contactPhone": "0123456789",
      "contactEmail": "contact@evcs.com",
      "description": "Main charging station",
      "isActive": true,
      "imageURL": "https://example.com/image.jpg",
      "createdDate": "2024-01-01T00:00:00",
      "modifiedDate": "2024-01-02T00:00:00"
    }
  ]
}
```

#### 2. GetByIdAsync
**Method:** `GetByIdAsync`  
**Request Type:** `StationRequest`  
**Response Type:** `Station`

**Request Body:**
```json
{
  "stationId": 1
}
```

#### 3. GetActiveStationsAsync
**Method:** `GetActiveStationsAsync`  
**Request Type:** `EmptyRequest`  
**Response Type:** `StationList`

**Request Body:**
```json
{}
```

#### 4. SearchStationsAsync
**Method:** `SearchStationsAsync`  
**Request Type:** `SearchRequest`  
**Response Type:** `StationList`

**Request Body:**
```json
{
  "searchTerm": "Ho Chi Minh"
}
```

#### 5. CreateAsync
**Method:** `CreateAsync`  
**Request Type:** `CreateStationRequest`  
**Response Type:** `MutationRelay`

**Request Body:**
```json
{
  "stationCode": "ST002",
  "stationName": "New Station",
  "address": "456 Second St",
  "city": "Hanoi",
  "province": "Hanoi",
  "latitude": 21.028511,
  "longitude": 105.804817,
  "capacity": 5,
  "currentAvailable": 5,
  "owner": "EVCS Company",
  "contactPhone": "0987654321",
  "contactEmail": "hanoi@evcs.com",
  "description": "New charging station in Hanoi",
  "imageURL": "https://example.com/hanoi.jpg"
}
```

**Response:**
```json
{
  "result": 2
}
```

#### 6. UpdateAsync
**Method:** `UpdateAsync`  
**Request Type:** `Station`  
**Response Type:** `MutationRelay`

**Request Body:**
```json
{
  "stationId": 1,
  "stationCode": "ST001",
  "stationName": "Updated Station Name",
  "address": "123 Main St",
  "city": "Ho Chi Minh",
  "province": "Ho Chi Minh",
  "latitude": 10.762622,
  "longitude": 106.660172,
  "capacity": 10,
  "currentAvailable": 7,
  "owner": "EVCS Company",
  "contactPhone": "0123456789",
  "contactEmail": "contact@evcs.com",
  "description": "Updated description",
  "isActive": true,
  "imageURL": "https://example.com/image.jpg"
}
```

**Response:**
```json
{
  "result": 1
}
```

#### 7. DeleteAsync
**Method:** `DeleteAsync`  
**Request Type:** `StationRequest`  
**Response Type:** `Mutation`

**Request Body:**
```json
{
  "stationId": 1
}
```

**Response:**
```json
{
  "result": true
}
```

#### 8. ActivateAsync
**Method:** `ActivateAsync`  
**Request Type:** `StationRequest`  
**Response Type:** `Mutation`

**Request Body:**
```json
{
  "stationId": 1
}
```

**Response:**
```json
{
  "result": true
}
```

#### 9. UpdateAvailabilityAsync
**Method:** `UpdateAvailabilityAsync`  
**Request Type:** `UpdateAvailabilityRequest`  
**Response Type:** `Mutation`

**Request Body:**
```json
{
  "stationId": 1,
  "currentAvailable": 6
}
```

**Response:**
```json
{
  "result": true
}
```

## Testing Steps

### Step 1: Start the gRPC Service
```bash
cd EVCS.GrpcService.TriNM
dotnet run
```

### Step 2: Test in Postman
1. Open Postman
2. Create a new gRPC request
3. Set URL to `http://localhost:5112`
4. Select method from the dropdown (e.g., `StationGRPC.GetAllAsync`)
5. Enter request body in JSON format
6. Click **Invoke**

## Common Issues

### Issue: Connection Refused
**Solution:** Ensure the gRPC service is running on port 5112

### Issue: Method Not Found
**Solution:** 
- Verify proto files are imported correctly
- Check that the service name matches: `StationGRPC`
- Ensure method names match exactly (case-sensitive)

### Issue: Invalid Request Format
**Solution:**
- Verify JSON structure matches the proto message definition
- Check that required fields are provided
- Ensure data types match (e.g., numbers not strings)

## Proto File Structure

The service uses the following proto definitions:
- **Service:** `StationGRPC`
- **Package:** `EVCS.GrpcService.TriNM.Protos`
- **Messages:** `Station`, `StationList`, `StationRequest`, `CreateStationRequest`, `SearchRequest`, `UpdateAvailabilityRequest`, `EmptyRequest`, `Mutation`, `MutationRelay`

## Notes
- All timestamps are returned as strings in ISO format
- Boolean values use `true`/`false`
- Empty strings are allowed for optional fields
- The service performs soft delete (sets `IsActive = false`) by default

