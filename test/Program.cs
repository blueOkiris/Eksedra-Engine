/*
 * This is a user side application.
 * After adding my library, they simply create the engine, and run it.
 * Engine takes window size, title, start room, and custom objects that room files contain"
 */
using System.Collections.Generic;
using SFML.System;
using EksedraEngine;
using System;
using System.Runtime.InteropServices;

namespace test {
    class Program {
        #if _WINDOWS
        #else
            // Must happen on linux or else threads will crash X11
            [DllImport("X11")]
            extern public static int XInitThreads();
        #endif
        
        static void Main(string[] args) {
            #if _WINDOWS
            #else
                // Required for threads on linux to work
                // Must be called before anything else not just before window unfotunately.
                // Thus can't be part of lib
                XInitThreads();
            #endif

            Console.WriteLine("Program started!");

            Engine engine = new Engine(
                                1280, 720, "Eksedra Engine", "test", 
                                new List<Type>() {
                                    typeof(ControlObject),
                                    typeof(Player),
                                    typeof(Rock),
                                    typeof(JumpThrough)
                                });

            Console.WriteLine("Running engine!");
            engine.Run();
        }
    }
}
