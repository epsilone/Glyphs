/*
// Use Parse.Cloud.define to define loud functions.
Parse.Cloud.define("hello", function(request, response) {
  response.success("Hello world1! ");
});*/
 
// before saving a multiplayer sequence, trying to find the opponent and notify him.
Parse.Cloud.beforeSave("MultiplayerSequence", function(request, response) {
 
if(request.object.get("sequence").length!=0){
     
    if(request.object.get("playing_user")==null){
        console.error("no playing_user. Is this a random challenge waiting for an opponent, or a real error?");
        response.success();
        return;
    }
     
  var opponentQuery = new Parse.Query(Parse.User);
   
  opponentQuery.get(request.object.get("waiting_user").id, {
    success: function (op) {
         
      var op_name = op.get("name");
 
     
      var userQuery = new Parse.Query(Parse.User)
 
      userQuery.get(request.object.get("playing_user").id, {
        success: function (user) {
          var registration_id = user.get("registration_id");
          var deviceToken = user.get("deviceToken");
          var turn = request.object.get("turn_number");
 
          var content_title = "Glyphs";
          var content_text;// = "New Challenge From " + op_name;
          var ticker;// = "New Challenge From " + op_name;
 
          if (turn <= 1) {
            var content_title = "Glyphs";
            var content_text = "New Challenge From " + op_name;
            var ticker = "New Challenge From " + op_name;
          }
        else{
            content_title = "Glyphs";
            content_text = "It's Your Turn Against " + op_name;
            ticker = "It's Your Turn Against " + op_name;
        }
 
        if(registration_id!=null) //android
        {
          Parse.Cloud.httpRequest({
            method: 'POST',
            url: "https://android.googleapis.com/gcm/send",
            headers: {
              'Authorization': 'key=AIzaSyAUNxNIRFsGZDD00TYvPPcwP3KXBAZuKJE',
              'Content-Type': 'application/json;charset=utf-8'
            },
            body: {
              'registration_ids': [registration_id],
              'data': {'content_title': content_title,
              'content_text': content_text,
              'ticker': ticker}
            },
            success: function(httpResponse) {
              response.success();
            },
            error: function(httpResponse) {
              console.error("error while sending push notification");
              console.error('Request failed with response code ' + httpResponse.status);
              console.error(httpResponse);
              response.success(); //still let us save the pattern
              }
            });
        }
        else if (deviceToken !=null) //ios
        {
            var pushQuery = new Parse.Query(Parse.Installation);
              pushQuery.equalTo('deviceToken', deviceToken);
 
              Parse.Push.send({
                where: pushQuery, // Set our Installation query
                data: {
                  alert: content_text
                }
              }, {
                success: function() {
                  // Push was successful
                    response.success();
                },
                error: function(error) {
                 console.error("error while sending push notification");
                  throw "Got an error " + error.code + " : " + error.message;
                response.success(); //still let us save the pattern
                }
              });
        }
            else// no push notification
            {
                  response.success(); //still let us save the pattern
            }
          },
          error : function (error) {
 
              console.log("unable to retrieve parse user with id: " + request.object.get("playing_user").id);
              //response.success();
 
          }
        });
         
    }});
       
}
    else{
    console.error('pattern length 0');
    }
});
 
// after saving a multiplayer sequence, update the activity of the player.
Parse.Cloud.afterSave("MultiplayerSequence", function(request, response) {
 
    var d = new Date();
    var currentLogin = new Date(d.getFullYear(), d.getMonth(), d.getDate())
    var consecutiveLogins=0;
    save = true;

    user = request.object.get("waiting_user");
    userConsecutiveLogins = user.get("consecutiveLogins");
    // the user never connected yet
    if (!userConsecutiveLogins)
    {
      // user never loged in.
      consecutiveLogins = 1;
    }
    else
    {
      // user loged, we need to compare with date
      userLastLogin = user.get("lastLogin");
      if (userLastLogin == currentLogin)
      {
        // login within the same day, don't need to save again.
        save = false;
      }
      else
      {
        var lastLoginPlusOneDay = new Date();
        lastLoginPlusOneDay = new Date(userLastLogin.getFullYear(), userLastLogin.getMonth(), userLastLogin.getDate() + 1)

      // the last login of the user has to 
      if (lastLoginPlusOneDay < currentLogin)
      {
        // the user didn't login within one day, resetting counter.
        consecutiveLogins = 1;
      }
      else
      {
        consecutiveLogins = user.consecutiveLogins + 1;
        // login within the same day.
        save = false;
      }
    }
  }

  if (save)
  {
    var userQuery = new Parse.Query(Parse.User);
     
    userQuery.get(request.object.get("waiting_user").id, {
      success: function (user) {
          user.set("lastLogin", currentLogin);
          user.set("consecutiveLogins", consecutiveLogins);
          user.save();
          console.log("success");
      },
    error: function(error){
        console.error(error);
    }});
  }
});

Parse.Cloud.afterSave("FinishedGame", function(request, response) {
    var longestPattern=0;
    var longestPatternThisWeek=0;
    var wins=0;
    var winsThisWeek=0;
     
    var longestPatternQuery = new Parse.Query(Parse.Object.extend("FinishedGame"));
    longestPatternQuery.equalTo("user",request.object.get("user"));
    longestPatternQuery.descending("patternLength");
    longestPatternQuery.first({
      success: function(object) {
          if(object != null)
            longestPattern=object.get("patternLength");
      }});
     
     
    var d = new Date();
    var time = (7 * 24 * 3600 * 1000);
    var aWeekAgo = new Date(d.getTime() - (time));
 
    var longestPatternQuery2 = new Parse.Query(Parse.Object.extend("FinishedGame"));
    longestPatternQuery2.equalTo("user",request.object.get("user"));
    longestPatternQuery2.descending("patternLength");
    longestPatternQuery2.greaterThanOrEqualTo( "createdAt", aWeekAgo );
     
    longestPatternQuery2.first({
      success: function(object) {
          if(object != null)
            longestPatternThisWeek=object.get("patternLength");
      }});
     
    var winsQuery = new Parse.Query(Parse.Object.extend("FinishedGame"));
    winsQuery.equalTo("user",request.object.get("user"));
    winsQuery.count({
      success: function(count) {
        wins=count;
      }});
     
    winsQuery.greaterThanOrEqualTo( "createdAt", aWeekAgo );
    winsQuery.count({
      success: function(count) {
        winsThisWeek=count;
      }});
    
Parse.Cloud.useMasterKey()
    
var winnerQuery = new Parse.Query(Parse.User);
   
  winnerQuery.get(request.object.get("user").id, {
    success: function (user) {
        user.set("longestPattern", longestPattern);
        user.set("longestPatternThisWeek", longestPatternThisWeek);
        user.set("wins", wins);
        user.set("winsThisWeek", winsThisWeek);
        user.set("artifacts",user.get("artifacts")+request.object.get("winReward"));
        user.save();
        console.log("saved winner");
    },
  error: function(error){
      console.error(error);
  }});
    
if(request.object.get("opponent")!=null)
{
    
var loserQuery = new Parse.Query(Parse.User);
   
  loserQuery.get(request.object.get("opponent").id, {
    success: function (user) {
       user.increment("artifacts",user.get("artifacts")+request.object.get("loseReward"));
        user.save();
        console.log("saved loser");
    },
  error: function(error){
      console.error(error);
  }});
}
    
});