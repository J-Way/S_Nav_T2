﻿using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

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

            //Start of first floor rooms
            rooms.Add("E100");
            rooms.Add("E101");
            rooms.Add("E101A");
            rooms.Add("E101B");
            rooms.Add("E101C");
            rooms.Add("E101D");
            rooms.Add("E102");
            rooms.Add("E103");
            rooms.Add("E104");
            rooms.Add("E105");
            rooms.Add("E106");
            rooms.Add("E108");
            rooms.Add("E108A");
            rooms.Add("E108B");
            rooms.Add("E108C");
            rooms.Add("E108D");
            rooms.Add("E109");
            rooms.Add("E110");
            rooms.Add("E110A");
            rooms.Add("E110B");
            rooms.Add("E111");
            rooms.Add("E113");
            rooms.Add("E114");//22 rooms

            rooms.Sort();

            return rooms;
        }
        
        // Husk of a function
        // 
        public List<MapPoint> loadPoints(int width, int height)
        {
            List<MapPoint> points = new List<MapPoint>();

            // floor traversal
            points.Add(new MapPoint("stairsTopLeft", new SKPoint(width * .18f, height * .20f)));//topleft stairs 
            points.Add(new MapPoint("stairsTopRight", new SKPoint(width * .885f, height * .30f))); //topRight stairs
            points.Add(new MapPoint("stairsBottomLeft", new SKPoint(width * .36f, height * .96f))); //bottom left stairs

            //Bathrooms
            points.Add(new MapPoint("bathroomGirls", new SKPoint(width * .48f, height * .52f))); //girls bathroom
            points.Add(new MapPoint("bathroomHandicap", new SKPoint(width * .78f, height * .32f)));  //handicap bathroom
            points.Add(new MapPoint("bathroomMen", new SKPoint(width * .6f, height * .50f))); //mens bathroom

            return points;
        }

        public List<List<MapPoint>> loadE2Points(int width, int height)
        {
            List<List<MapPoint>> mapPoints = new List<List<MapPoint>>();
            List<MapPoint> roomPoints = new List<MapPoint>();
            List<MapPoint> hallPoints = new List<MapPoint>();
            List<MapPoint> traversalPoints = new List<MapPoint>();

            mapPoints.Add(roomPoints);
            mapPoints.Add(traversalPoints);
            mapPoints.Add(hallPoints);

            //
            // non-room points
            // floor traversal
            traversalPoints.Add(new MapPoint("stairsTopLeft", new SKPoint(width * .23f, height * .26f)));//topleft stairs 
            traversalPoints.Add(new MapPoint("stairsTopRight", new SKPoint(width * .885f, height * .30f))); //topRight stairs
            traversalPoints.Add(new MapPoint("stairsBottomLeft", new SKPoint(width * .36f, height * .96f))); //bottom left stairs
            traversalPoints.Add(new MapPoint("stairsMiddle", new SKPoint(width * .525f, height * .54f))); 

            // hallway
            hallPoints.Add(new MapPoint("hallTopLeft", new SKPoint(width * .37f, height * .30f)));
            hallPoints.Add(new MapPoint("hallMidLeftCenter", new SKPoint(width * .37f, height * .53f)));
            hallPoints.Add(new MapPoint("hallE207Entrance", new SKPoint(width * .37f, height * .60f)));
            hallPoints.Add(new MapPoint("hallLowereft", new SKPoint(width * .37f, height * .73f)));
            hallPoints.Add(new MapPoint("hallBottomLeft", new SKPoint(width * .37f, height * .90f)));
            hallPoints.Add(new MapPoint("hallTopMid", new SKPoint(width * .50f, height * .30f)));
            hallPoints.Add(new MapPoint("hallTopRight", new SKPoint(width * .73f, height * .30f)));
            hallPoints.Add(new MapPoint("hallCurvedStart", new SKPoint(width * .73f, height * .36f)));
            hallPoints.Add(new MapPoint("hallCurvedMid", new SKPoint(width * .68f, height * .43f)));
            hallPoints.Add(new MapPoint("hallBottomRight", new SKPoint(width * .57f, height * .53f)));

            //
            // ROOM POINTS FOR SECOND FLOOR
            roomPoints.Add(new MapPoint("E200", new SKPoint(width * .50f, height * .38f))); //E200 (Center left)
            roomPoints.Add(new MapPoint("E200A", new SKPoint(width * .67f, height * .36f))); //E200A
            roomPoints.Add(new MapPoint("E200B", new SKPoint(width * .67f, height * .36f))); //E200B
            roomPoints.Add(new MapPoint("E200C", new SKPoint(width * .62f, height * .36f))); //E200C
            roomPoints.Add(new MapPoint("E200D", new SKPoint(width * .57f, height * .36f))); //E200D
            roomPoints.Add(new MapPoint("E200E", new SKPoint(width * .555f, height * .36f))); //E200E
            roomPoints.Add(new MapPoint("E200F", new SKPoint(width * .50f, height * .36f))); //E200F
            roomPoints.Add(new MapPoint("E200G", new SKPoint(width * .50f, height * .42f))); //E200G
            roomPoints.Add(new MapPoint("E200H", new SKPoint(width * .50f, height * .44f))); //E200H
            roomPoints.Add(new MapPoint("E200I", new SKPoint(width * .525f, height * .44f))); //E200I
            roomPoints.Add(new MapPoint("E200J", new SKPoint(width * .55f, height * .44f))); //E200J
            roomPoints.Add(new MapPoint("E200K", new SKPoint(width * .58f, height * .44f))); //E200K
            //
            roomPoints.Add(new MapPoint("E202", new SKPoint(width * .40f, height * .54f))); // E202
            roomPoints.Add(new MapPoint("E203", new SKPoint(width * .385f, height * .74f))); //E203
            roomPoints.Add(new MapPoint("E204", new SKPoint(width * .385f, height * .84f))); //E204
            roomPoints.Add(new MapPoint("E205", new SKPoint(width * .385f, height * .94f))); //E205
            roomPoints.Add(new MapPoint("E206", new SKPoint(width * .36f, height * .87f))); //E206
            //E207
            roomPoints.Add(new MapPoint("E207", new SKPoint(width * .27f, height * .60f))); //E207
            roomPoints.Add(new MapPoint("E207A", new SKPoint(width * .275f, height * .56f))); //E207A
            roomPoints.Add(new MapPoint("E207B", new SKPoint(width * .275f, height * .63f))); //E207B
            roomPoints.Add(new MapPoint("E207C", new SKPoint(width * .36f, height * .76f))); //E207C
            roomPoints.Add(new MapPoint("E207D", new SKPoint(width * .275f, height * .63f))); //E207D
            roomPoints.Add(new MapPoint("E207E", new SKPoint(width * .25f, height * .60f))); //E207E
            roomPoints.Add(new MapPoint("E207F", new SKPoint(width * .26f, height * .54f))); //E207F
            roomPoints.Add(new MapPoint("E207G", new SKPoint(width * .275f, height * .555f))); //E207G
            roomPoints.Add(new MapPoint("E207U", new SKPoint(width * .25f, height * .60f))); //E207U
            //
            roomPoints.Add(new MapPoint("E208", new SKPoint(width * .265f, height * .31f))); // E208
            roomPoints.Add(new MapPoint("E209", new SKPoint(width * .265f, height * .31f))); // E209
            roomPoints.Add(new MapPoint("E210", new SKPoint(width * .385f, height * .42f))); //E210
            roomPoints.Add(new MapPoint("E211", new SKPoint(width * .385f, height * .50f))); //E211
            roomPoints.Add(new MapPoint("E212", new SKPoint(width * .42f, height * .52f))); //E212

            return mapPoints;
        }


        public List<List<MapPoint>> loadE1Points(int width, int height)
        {
            List<List<MapPoint>> mapPoints = new List<List<MapPoint>>();
            List<MapPoint> roomPoints = new List<MapPoint>();
            List<MapPoint> hallPoints = new List<MapPoint>();
            List<MapPoint> traversalPoints = new List<MapPoint>();

            mapPoints.Add(roomPoints);
            mapPoints.Add(traversalPoints);
            mapPoints.Add(hallPoints);

            //
            // non-room points

            // floor traversal
            traversalPoints.Add(new MapPoint("stairsTopLeft", new SKPoint(width * .18f, height * .20f)));//topleft stairs 
            traversalPoints.Add(new MapPoint("stairsTopRight", new SKPoint(width * .885f, height * .30f))); //topRight stairs
            traversalPoints.Add(new MapPoint("stairsBottomLeft", new SKPoint(width * .315f, height * .93f))); //bottom left stairs
            traversalPoints.Add(new MapPoint("stairsMiddle", new SKPoint(width * .515f, height * .52f)));

            //hallway points
            hallPoints.Add(new MapPoint("hallFarLeft", new SKPoint(width * .18f, height * .24f))); //adjust
            hallPoints.Add(new MapPoint("hallTopLeft", new SKPoint(width * .345f, height * .24f))); //adjust
            hallPoints.Add(new MapPoint("hallTopMidLeft", new SKPoint(width * .42f, height * .24f))); //added
            hallPoints.Add(new MapPoint("hallTopMid", new SKPoint(width * .57f, height * .24f))); //adjusted
            hallPoints.Add(new MapPoint("hallTopRight", new SKPoint(width * .74f, height * .24f))); //adjusted
            hallPoints.Add(new MapPoint("hallMidLeftUpper", new SKPoint(width * .345f, height * .35f))); //added and adjusted
            hallPoints.Add(new MapPoint("hallMidLeftCenter", new SKPoint(width * .345f, height * .485f))); //adjusted
            hallPoints.Add(new MapPoint("hallE207Entrance", new SKPoint(width * .345f, height * .60f))); //adjusted
            
            hallPoints.Add(new MapPoint("hallE110Inside", new SKPoint(width * .295f, height * .32f))); //added, not functional
            hallPoints.Add(new MapPoint("hallE110Door", new SKPoint(width * .31f, height * .385f))); //added, not functional

            hallPoints.Add(new MapPoint("hallMidBottomLeft", new SKPoint(width * .345f, height * .85f))); //added and adjusted
            hallPoints.Add(new MapPoint("hallMidHalfBottomLeft", new SKPoint(width * .345f, height * .79f))); //added and adjusted
            hallPoints.Add(new MapPoint("hallBottomLeft", new SKPoint(width * .345f, height * .73f))); //adjusted
            hallPoints.Add(new MapPoint("hallFarBottomLeft", new SKPoint(width * .345f, height * .93f))); 

            hallPoints.Add(new MapPoint("hallCurvedStart", new SKPoint(width * .75f, height * .31f))); //adjusted
            hallPoints.Add(new MapPoint("hallCurvedMid", new SKPoint(width * .67f, height * .39f))); //adjusted
            hallPoints.Add(new MapPoint("hallBottomRight", new SKPoint(width * .57f, height * .485f))); //adjusted

            hallPoints.Add(new MapPoint("hallE108UpperInside", new SKPoint(width * .22f, height * .595f))); //added, not really functional
            hallPoints.Add(new MapPoint("hallE108LowerInside", new SKPoint(width * .22f, height * .695f))); //added, not really functional

            //Start of first floor points
            roomPoints.Add(new MapPoint("E100", new SKPoint(width * .71f, height * .32f))); //done
            roomPoints.Add(new MapPoint("E101", new SKPoint(width * .575f, height * .268f))); //done 
            roomPoints.Add(new MapPoint("E101A", new SKPoint(width * .475f, height * .405f))); //done
            roomPoints.Add(new MapPoint("E101B", new SKPoint(width * .37f, height * .39f))); //done
            roomPoints.Add(new MapPoint("E101C", new SKPoint(width * .40f, height * .46f))); //done 
            roomPoints.Add(new MapPoint("E101D", new SKPoint(width * .37f, height * .365f))); //done
            roomPoints.Add(new MapPoint("E102", new SKPoint(width * .455f, height * .51f))); //done
            roomPoints.Add(new MapPoint("E103", new SKPoint(width * .365f, height * .7f))); //done
            roomPoints.Add(new MapPoint("E104", new SKPoint(width * .365f, height * .795f))); //done
            roomPoints.Add(new MapPoint("E105", new SKPoint(width * .365f, height * .89f))); //done
            roomPoints.Add(new MapPoint("E106", new SKPoint(width * .32f, height * .83f))); //done
            roomPoints.Add(new MapPoint("E108", new SKPoint(width * .32f, height * .595f))); //done
            roomPoints.Add(new MapPoint("E108A", new SKPoint(width * .295f, height * .785f))); //done
            roomPoints.Add(new MapPoint("E108B", new SKPoint(width * .275f, height * .785f))); //done
            roomPoints.Add(new MapPoint("E108C", new SKPoint(width * .29f, height * .585f))); //done
            roomPoints.Add(new MapPoint("E108D", new SKPoint(width * .29f, height * .61f))); //done
            roomPoints.Add(new MapPoint("E109", new SKPoint(width * .32f, height * .425f))); //done
            roomPoints.Add(new MapPoint("E110", new SKPoint(width * .32f, height * .385f))); //done
            roomPoints.Add(new MapPoint("E110A", new SKPoint(width * .25f, height * .385f))); //done
            roomPoints.Add(new MapPoint("E110B", new SKPoint(width * .275f, height * .285f))); //done
            roomPoints.Add(new MapPoint("E111", new SKPoint(width * .2f, height * .26f))); //done
            roomPoints.Add(new MapPoint("E113", new SKPoint(width * .51f, height * .4555f))); //done
            roomPoints.Add(new MapPoint("E114", new SKPoint(width * .79f, height * .24f))); //done

            return mapPoints;
        }

        public List<string> loadWingNames()
        {
            List<string> names = new List<string>();

            // replace these with more informative names 
            // when porting to firebase
            names.Add("E-1");
            names.Add("E-2");

            names.Add("G-B");
            names.Add("G-1");
            names.Add("G-2");
            names.Add("G-3");
            names.Add("G-4");

            names.Sort();

            return names;
        }
    }
}