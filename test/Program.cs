/*
 * This is a user side application.
 * After adding my library, they simply create the engine, and run it.
 * Engine takes window size, title, start room, and custom objects that room files contain"
 */
using System.Collections.Generic;
using SFML.System;
using EksedraEngine;
using System;

namespace test {
    class Program {
        static void Main(string[] args) {
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
