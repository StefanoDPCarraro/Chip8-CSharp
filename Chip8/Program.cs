using Chip8_CSharp.Chip8;
using SDL2;

public class Program{
    static void Main(){
        bool open = false;

        ProcessingUnit cpu = new ProcessingUnit();

        SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING);
        IntPtr window = SDL.SDL_CreateWindow("CHIP8", 100, 100, 640, 480, SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
        IntPtr nomequetuquiser = SDL.SDL_CreateRenderer(window,-1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

        SDL.SDL_Event evento;
        open = true;
        

        while(open){
            SDL.SDL_PollEvent(out evento);
            if(evento.type == SDL.SDL_EventType.SDL_QUIT){
                open = false;
            }
            if(evento.type == SDL.SDL_EventType.SDL_KEYDOWN){
                switch (evento.key.keysym.sym){
                    case SDL.SDL_Keycode.SDLK_1:
                        cpu.Keyboard = 0x01;
                        Console.WriteLine(cpu.Keyboard);
                        break;
                    
                    case SDL.SDL_Keycode.SDLK_2:
                        cpu.Keyboard = 0x02;
                        Console.WriteLine(cpu.Keyboard);
                        break;

                    case SDL.SDL_Keycode.SDLK_3:
                        cpu.Keyboard = 0x03;
                        Console.WriteLine(cpu.Keyboard);
                        break;

                    case SDL.SDL_Keycode.SDLK_4:
                        cpu.Keyboard = 0x0C;
                        Console.WriteLine(cpu.Keyboard);
                        break;

                    case SDL.SDL_Keycode.SDLK_q:
                        cpu.Keyboard = 0x04;
                        Console.WriteLine(cpu.Keyboard);
                        break;

                    case SDL.SDL_Keycode.SDLK_w:
                        cpu.Keyboard = 0x05;
                        Console.WriteLine(cpu.Keyboard);
                        break;

                    case SDL.SDL_Keycode.SDLK_e:
                        cpu.Keyboard = 0x06;
                        Console.WriteLine(cpu.Keyboard);
                        break;

                    case SDL.SDL_Keycode.SDLK_r:
                        cpu.Keyboard = 0x0D;
                        Console.WriteLine(cpu.Keyboard);
                        break;

                    case SDL.SDL_Keycode.SDLK_a:
                        cpu.Keyboard = 0x07;
                        Console.WriteLine(cpu.Keyboard);
                        break;

                    case SDL.SDL_Keycode.SDLK_s:
                        cpu.Keyboard = 0x08;
                        Console.WriteLine(cpu.Keyboard);
                        break;

                    case SDL.SDL_Keycode.SDLK_d:
                        cpu.Keyboard = 0x09;
                        Console.WriteLine(cpu.Keyboard);
                        break;

                    case SDL.SDL_Keycode.SDLK_f:
                        cpu.Keyboard = 0x0E;
                        Console.WriteLine(cpu.Keyboard);
                        break;

                    case SDL.SDL_Keycode.SDLK_z:
                        cpu.Keyboard = 0x0A;
                        Console.WriteLine(cpu.Keyboard);
                        break;

                    case SDL.SDL_Keycode.SDLK_x:
                        cpu.Keyboard = 0x00;
                        Console.WriteLine(cpu.Keyboard);
                        break;

                    case SDL.SDL_Keycode.SDLK_c:
                        cpu.Keyboard = 0x0B;
                        Console.WriteLine(cpu.Keyboard);
                        break;

                    case SDL.SDL_Keycode.SDLK_v:
                        cpu.Keyboard = 0x0F;
                        Console.WriteLine(cpu.Keyboard);
                        break;
                }
            }
        }
    }
}
