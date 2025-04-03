using System;

namespace Chip8_CSharp.Chip8
{
    class ProcessingUnit
    {
        byte[] mem = new byte[4096]; // 4k bytes memory
        byte[] V = new byte[16]; // Registers (addr F reserved for carry flag)
        ushort[] stack = new ushort[48];
        ushort SP = 0;
        ushort I = 0;
        ushort PC = 0x200; // As described should start at 0x200 as the prior addr are reserved
        byte Vx = 0;
        byte Vy = 0;

        void runCpu()
        {
            while(true)
            {
                ushort opcode = (ushort)(mem[PC] << 8 | mem[PC + 1]); // Read opcode (stored in 2 bytes)
                opcodeCases(opcode);
                PC += 2;
            }
        }

        void opcodeCases(ushort opcode)
        {
            switch(opcode & 0xF000) //Ignores the 3 least significant bytes
            {
                case 0x0000:
                    if(opcode == 0x00E0)
                    {
                        clearScreen();
                    }
                    else if(opcode == 0x00EE)
                    {
                        // Returns subroutine
                        SP -= 1;
                        PC = SP;
                    }
                    break;

                case 0x1000:
                    // 1NNN
                    // Jumps to NNN
                    PC = (ushort)(opcode & 0x0FFF);
                    break;
                
                case 0x2000:
                    // 2NNN
                    // Calls subroutine at NNN
                    stack[SP] = PC;
                    SP++;
                    PC = (ushort)(opcode & 0x0FFF);
                    break;

                case 0x3000:
                    // 3XNN
                    // Skips next instruction if V[x] == NN
                    Vx = V[(byte)(opcode & 0x0F00 >> 16)];
                    if((byte)(opcode & 0x00FF) == Vx)
                    {
                        PC += 2;
                    }
                    break;

                case 0x4000:
                    // 4XNN
                    // Skips next instruction if V[x] != NN
                    Vx = V[(byte)(opcode & 0x0F00 >> 16)];
                    if((byte)(opcode & 0x00FF) != Vx)
                    {
                        PC += 2;
                    }
                    break;

                case 0x5000:
                    // 5XY0
                    // Skips next instruction if V[x] == V[y]
                    Vx = V[(byte)(opcode & 0x0F00) >> 16];
                    Vy = V[(byte)(opcode & 0x00F0) >> 8];
                    if( Vx == Vy)
                    {
                        
                    }
                    break;

                case 0x6000:
                    // 6XNN
                    // Vx = NN
                    V[(byte)(opcode & 0x0F00) >> 16] = (byte)(opcode & 0x00FF);
                    break;

                case 0x7000:
                    // 7XNN
                    // Vx += NN
                    V[(byte)(opcode & 0x0F00) >> 16] += (byte)(opcode & 0x00FF);
                    break;

                case 0x8000:
                    // Generalization
                    Vy = V[(byte)(opcode & 0x00F0) >> 8];

                    switch(opcode & 0xF00F)
                    {
                        case 0x8000:
                            // 8XY0
                            // Vx = Vy
                            V[(byte)(opcode & 0x0F00) >> 16] = Vy;
                            break;
                        
                        case 0x8001:
                            // 8XY1
                            // Vx |= Vy
                            V[(byte)(opcode & 0x0F00) >> 16] |= Vy;
                            break;

                        case 0x8002:
                            // 8XY2
                            // Vx &= Vy
                            V[(byte)(opcode & 0x0F00) >> 16] &= Vy;
                            break;

                        case 0x8003:
                            // 8XY3
                            // Vx ^= Vy
                            V[(byte)(opcode & 0x0F00) >> 16] ^= Vy;
                            break;
                        
                        case 0x8004:
                            // 8XY4
                            // Vx += Vy
                            // TODO: Check for overflow
                            V[(byte)(opcode & 0x0F00) >> 16] += Vy;
                            break;

                        case 0x8005:
                            // 8XY5
                            // Vx -= Vy
                            // TODO: Check for underflow
                            V[(byte)(opcode & 0x0F00) >> 16] -= Vy;
                            break;

                        case 0x8006:
                            // 8XY6
                            // Vx >>= 1
                            // TODO: Stores the least significant bit of VX prior to the shift into VF
                            V[(byte)(opcode & 0x0F00) >> 16] >>= 1;
                            break;

                    }
                    break;
            }
        }

        void clearScreen(){
            return;
        }
    }
}