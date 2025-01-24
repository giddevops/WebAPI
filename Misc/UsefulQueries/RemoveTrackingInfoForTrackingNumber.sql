
delete IncomingShipmentShipmentTrackingEvent from IncomingShipmentShipmentTrackingEvent
left join IncomingShipment on IncomingShipmentShipmentTrackingEvent.IncomingShipmentId = IncomingShipment.Id
WHERE TrackingNumber = '1Z2X07F40489973438';

Update IncomingShipment set ActualArrivalDate = null WHERE TrackingNumber='1Z2X07F40489973438';