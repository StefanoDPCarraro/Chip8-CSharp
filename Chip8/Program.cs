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
                        cpu.Keyboard = 1;
                        Console.WriteLine(cpu.Keyboard);
                        break;

                }
            }
        }
    }
}
