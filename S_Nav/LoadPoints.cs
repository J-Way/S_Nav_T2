using System;
using System.Collections.Generic;
using System.Numerics;

namespace S_Nav
{
    // will later include loading points from file, using hardcoded for now
    class LoadPoints
    {
        
        public List<String> loadRoomNames()
        {
            List<String> rooms = new List<string>();

            // E200
            rooms.Add("E200"); //E200 (Center left)
            rooms.Add("E200A"); //E200A
            rooms.Add("E200B"); //E200B
            rooms.Add("E200C"); //E200C
            rooms.Add("E200D"); //E200D
            rooms.Add("E200E"); //E200E
            rooms.Add("E200F"); //E200F
            rooms.Add("E200G"); //E200G
            rooms.Add("E200H"); //E200H
            rooms.Add("E200I"); //E200I
            rooms.Add("E200J"); //E200J
            rooms.Add("E200K"); //E200K

            //
            rooms.Add("E202"); // E202
            rooms.Add("E203"); //E203
            rooms.Add("E204"); //E204
            rooms.Add("E205"); //E205
            rooms.Add("E206"); //E206
            
            //E207
            rooms.Add("E207"); //E207
            rooms.Add("E207A"); //E207A
            rooms.Add("E207B"); //E207B
            rooms.Add("E207C"); //E207C
            rooms.Add("E207D"); //E207D
            rooms.Add("E207E"); //E207E
            rooms.Add("E207F"); //E207F
            rooms.Add("E207G"); //E207G
            rooms.Add("E207U"); //E207U

            //
            rooms.Add("E208"); // E208
            rooms.Add("E209"); // E209
            rooms.Add("E210"); //E210
            rooms.Add("E211"); //E211
            rooms.Add("E212"); //E212

            return rooms;
        }
        

        public List<MapPoint> loadPoints(int width, int height)
        {
            List<MapPoint> points = new List<MapPoint>();
            
            //hallway points
            points.Add(new MapPoint("hallTopLeft", new Vector2(width * .37f, height * .30f)));
            points.Add(new MapPoint("hallMidLeftCenter", new Vector2(width * .37f, height * .53f)));
            points.Add(new MapPoint("hallE207Entrance", new Vector2(width * .37f, height * .60f)));
            points.Add(new MapPoint("hallBottomLeft", new Vector2(width * .37f, height * .73f)));
            points.Add(new MapPoint("hallTopMid", new Vector2(width * .50f, height * .30f)));
            points.Add(new MapPoint("hallTopRight", new Vector2(width * .73f, height * .30f)));
            points.Add(new MapPoint("hallCurvedStart", new Vector2(width * .73f, height * .36f)));
            points.Add(new MapPoint("hallCurvedMid", new Vector2(width * .68f, height * .43f)));
            points.Add(new MapPoint("hallBottomRight", new Vector2(width * .57f, height * .53f)));

            
            // floor traversal
            points.Add(new MapPoint("stairsTopLeft", new Vector2(width * .24f, height * .25f)));//topleft stairs 
            points.Add(new MapPoint("elevatorTopLeft",new Vector2(width * .21f, height * .25f)));//topleft elevator
            points.Add(new MapPoint("stairsTopRight",new Vector2(width * .885f, height * .30f))); //topRight stairs
            points.Add(new MapPoint("stairsBottomLeft", new Vector2(width * .36f, height * .96f))); //bottom left stairs
            
            //Bathrooms
            points.Add(new MapPoint("bathroomGirls",new Vector2(width * .48f, height * .52f))); //girls bathroom
            points.Add(new MapPoint("bathroomHandicap", new Vector2(width * .78f, height * .32f)));  //handicap bathroom
            points.Add(new MapPoint("bathroomMen",new Vector2(width * .6f, height * .50f))); //mens bathroom

            // E200
            points.Add(new MapPoint("E200", new Vector2(width * .50f, height * .38f))); //E200 (Center left)
            points.Add(new MapPoint("E200A", new Vector2(width * .67f, height * .36f))); //E200A
            points.Add(new MapPoint("E200B", new Vector2(width * .67f, height * .36f))); //E200B
            points.Add(new MapPoint("E200C", new Vector2(width * .62f, height * .36f))); //E200C
            points.Add(new MapPoint("E200D", new Vector2(width * .57f, height * .36f))); //E200D
            points.Add(new MapPoint("E200E", new Vector2(width * .555f, height * .36f))); //E200E
            points.Add(new MapPoint("E200F", new Vector2(width * .50f, height * .36f))); //E200F
            points.Add(new MapPoint("E200G", new Vector2(width * .50f, height * .42f))); //E200G
            points.Add(new MapPoint("E200H", new Vector2(width * .50f, height * .44f))); //E200H
            points.Add(new MapPoint("E200I", new Vector2(width * .525f, height * .44f))); //E200I
            points.Add(new MapPoint("E200J", new Vector2(width * .55f, height * .44f))); //E200J
            points.Add(new MapPoint("E200K", new Vector2(width * .58f, height * .44f))); //E200K

            //
            points.Add(new MapPoint("E202", new Vector2(width * .40f, height * .54f))); // E202
            points.Add(new MapPoint("E203", new Vector2(width * .385f, height * .74f))); //E203
            points.Add(new MapPoint("E204", new Vector2(width * .385f, height * .84f))); //E204
            points.Add(new MapPoint("E205", new Vector2(width * .385f, height * .94f))); //E205
            points.Add(new MapPoint("E206", new Vector2(width * .36f, height * .87f))); //E206

            //E207
            points.Add(new MapPoint("E207", new Vector2(width * .27f, height * .60f))); //E207
            points.Add(new MapPoint("E207A", new Vector2(width * .275f, height * .56f))); //E207A
            points.Add(new MapPoint("E207B", new Vector2(width * .275f, height * .63f))); //E207B
            points.Add(new MapPoint("E207C", new Vector2(width * .36f, height * .76f))); //E207C
            points.Add(new MapPoint("E207D", new Vector2(width * .275f, height * .63f))); //E207D
            points.Add(new MapPoint("E207E", new Vector2(width * .25f, height * .60f))); //E207E
            points.Add(new MapPoint("E207F", new Vector2(width * .26f, height * .54f))); //E207F
            points.Add(new MapPoint("E207G", new Vector2(width * .275f, height * .555f))); //E207G
            points.Add(new MapPoint("E207U", new Vector2(width * .25f, height * .60f))); //E207U

            //
            points.Add(new MapPoint("E208", new Vector2(width * .265f, height * .31f))); // E208
            points.Add(new MapPoint("E209", new Vector2(width * .265f, height * .31f))); // E209
            points.Add(new MapPoint("E210", new Vector2(width * .385f, height * .42f))); //E210
            points.Add(new MapPoint("E211", new Vector2(width * .385f, height * .50f))); //E211
            points.Add(new MapPoint("E212", new Vector2(width * .42f, height * .52f))); //E212
            
            return points;
            
        }

        // loadFromFile()
        // loadFromServer()
    }
}
