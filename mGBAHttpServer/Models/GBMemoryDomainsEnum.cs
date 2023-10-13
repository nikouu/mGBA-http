namespace mGBAHttpServer.Models
{
    public enum GBMemoryDomainsEnum
    {
        cart0 = 0x0000, // ROM Bank
        vram = 0x8000, // VRAM
        sram = 0xa000, // SRAM
        wram = 0xc000, // WRAM
        oam = 0xfe00, // OAM
        io = 0xff00, // MMIO
        hram = 0xff80 // HRAM
    }
}
