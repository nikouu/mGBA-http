namespace mGBAHttpServer.Models
{
    public enum GBAMemoryDomainsEnum
    {
        bios = 0x00000000, // BIOS
        wram = 0x02000000, // EWRAM
        iwram = 0x03000000, // IWRAM
        io = 0x04000000, // MMIO
        palette = 0x05000000, // Palette
        vram = 0x06000000, // VRAM
        oam = 0x07000000, // OAM
        cart0 = 0x08000000, // ROM
        cart1 = 0x0a000000, // ROM WS1
        cart2 = 0x0c000000 // ROM WS2
    }
}
