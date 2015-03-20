/**
 * SetConfig (Custom API)
 * Set heartRate limits for calculate the Indicator.
 */

// FUNCTIONS
function setConfig(request, response, heartRateMin, heartRateMax){
    request.service.mssql.query(
        'UPDATE options SET value = \'' + heartRateMax + '\' WHERE name = \'heartRateMax\'', {
            success: function(results) {
                request.service.mssql.query(
                    'UPDATE options SET value = \'' + heartRateMin + '\' WHERE name = \'heartRateMin\'', {
                        success: function(results) {
                            response.send(statusCodes.OK, { heartRateMin: heartRateMin, heartRateMax: heartRateMax });
                        }
                    }
                );
            }
        }
    );
}

/**
 * GET
 */
exports.get = function(request, response) {
    if (request.query.heartRateMin !== null && request.query.heartRateMax !== null 
        && request.query.heartRateMin.length != 0 && request.query.heartRateMax.length != 0){
        var max = request.query.heartRateMax;
        var min = request.query.heartRateMin;
        setConfig(request, response, min, max);
    }
    else
    {
        response.send(400);
    }
};

/**
 * PUT
 */
exports.put = function(request, response) {
    if (request.body.heartRateMin !== null && request.body.heartRateMax !== null 
        && request.body.heartRateMin.length != 0 && request.body.heartRateMax.length != 0){
        var max = request.body.heartRateMax;
        var min = request.body.heartRateMax;
        setConfig(request, response, min, max);
    }
    else
    {
        response.send(400);
    }
};