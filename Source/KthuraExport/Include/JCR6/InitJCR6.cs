using UseJCR6;

static class InitJCR6 {
    public static void Go() {
        JCR6_lzma.Init();
        JCR6_jxsrcca.Init();
        JCR6_zlib.Init();
    }
}
