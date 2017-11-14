# Digital Twin

Creating Digital Twin using Actor Model and Azure Service Fabric

Prerequisite --

Visual studio and Azure service Fabric SDK
Azure subscription
Provision IoT Hub and Devices
You can use some device Simulator to generate messages.

Update variable to access IoT Hub -- 

File name -- DigitalTwinApp.TelemetryReceiver\TelemetryReceiver.cs
		private const string EhConnectionString = "";
        private const string EhEntityPath = "";
        private const string EhConsumerGroup = "";
        private const string StorageContainerName = "";
        private const string StorageAccountName = "";
        private const string StorageAccountKey = "";

Service Fabric can run locally. And Use Postman(DigitalTwinApi.postman_collection.json) collection to use APIs.

![alt text](https://github.com/Chittapriya/DigitalTwin/blob/master/Architecture.png)

For details refer <a href="https://chittapriya.wordpress.com/">Blog</a>