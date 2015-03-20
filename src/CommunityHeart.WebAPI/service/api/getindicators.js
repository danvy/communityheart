/**
 * GetIndicators (CustomApi)
 * Return last data from Database.
 * Add Heart Rate Indicator (1 = Low / 2 = Normal / 3 = High).
 */

exports.get = function(request, response) {
    request.service.mssql.query(
        'SELECT * FROM options WHERE name = \'heartRateMin\' OR name = \'heartRateMax\' ', {
            success: function(results) {
                var min = 0;
                var max = 0;
                
                if(results[0].name == "heartRateMin"){
                    min = results[0].value;
                    max = results[1].value;
                }
                else
                {
                    min = results[1].value;
                    max = results[0].value;
                }
                
                request.service.mssql.query(
                    'SELECT heartRate FROM heart WHERE heart.__createdAt = (SELECT MAX(__createdAt) FROM heart)', {
                        success: function(results) {
                            var bpm = results[0].heartRate;
                            var indicator = 0;
                                   
                            if(bpm < min)
                            {
                                // Trop lent
                                indicator = 1;
                            }
                            else if (bpm < max)
                            {
                                // Rythme normal
                                indicator = 2;
                            }
                            else
                            {
                                // Rythme BEAUCOUP TROP RAPIDE
                                indicator = 3;
                            }
                            
                            var finalJson = {
                                heartRate: bpm,
                                heartRateIndicator: indicator
                            }
                            
                            response.send(200, finalJson);
                        }
                    }
                );
            }
        }
    );
};