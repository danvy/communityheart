/**
 * SetValues (CustomApi)
 * Push a new heartRate Value to the Database
 */

function setBPM(request, response, heartRate){
    request.service.mssql.query(
        'INSERT INTO heart (heartRate) VALUES (' + heartRate + ')', {
            success: function(results) {
                response.send(statusCodes.OK, { heartRate : heartRate });
            }
        }
    );
}

exports.get = function(request, response) {
    if(request.query.heartRate !== null && request.query.heartRate.length != 0){
        var heartRate = request.query.heartRate;
        setBPM(request, response, heartRate);
    }
    else
    {
         response.send(400);
    }
};

exports.put = function(request, response) {
    if(request.body.heartRate !== null && request.body.heartRate.length != 0){
        var heartRate = request.body.heartRate;
        setBPM(request, response, heartRate);
    }
    else
    {
         response.send(400);
    }
};