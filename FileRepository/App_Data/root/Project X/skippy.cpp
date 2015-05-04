gettimeofday(&start, NULL);
        lseek_ret = (int)lseek(fd, i * SECTOR_SIZE, SEEK_CUR);
        assert(lseek_ret != -1);
        read_ret = read(fd, buffer, SECTOR_SIZE);
        assert(read_ret != -1);
        gettimeofday(&end, NULL);
        elapsed = (end.tv_sec - start.tv_sec) * 1000.0;
        elapsed += (end.tv_usec - start.tv_usec) / 1000.0;
        printf("measurement: %d, time: %lf ms\n", i, elapsed);