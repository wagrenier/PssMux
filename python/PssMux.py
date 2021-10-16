import io
from shutil import copyfile
import argparse

audio_segment = b'\x00\x00\x01\xBD'
pack_start = b'\x00\x00\x01\xBA'
end_file = b'\x00\x00\x01\xB9'

first_header_size = 0x3f
header_size = 0x17


def seek_next_audio(file):
    while True:
        block_id = file.read(0x4)

        if block_id == pack_start:
            file.seek(0xa, 1)
        elif block_id == audio_segment:
            return False
        elif block_id == end_file:
            return True
        else:
            block_size = file.read(0x2)
            file.seek(int.from_bytes(block_size, 'big'), 1)


def initial_audio_block(file):
    b_size = int.from_bytes(file.read(0x2), 'big')

    file.seek(0x3b - 0x6, 1)

    audio_total_size = int.from_bytes(file.read(0x4), 'little')

    data_size = b_size - first_header_size + 0x6

    print(audio_total_size)
    print(data_size)

    return [audio_total_size, data_size]


def audio_block(file):
    b_size = int.from_bytes(file.read(0x2), 'big')

    file.seek(-0x6, 1)
    file.seek(header_size, 1)

    data_size = b_size - header_size + 0x6

    print(data_size)

    return data_size


def build_full_audio_buffer(file):
    seek_next_audio(file)
    total_size, curr_block_size = initial_audio_block(file)
    buff = []
    buff += file.read(curr_block_size)

    while True:
        done = seek_next_audio(file)

        if done:
            break

        curr_block_size = audio_block(file)

        buff += file.read(curr_block_size)

    file.seek(0)
    return buff


def pss_mux(source: str, target: str):
    target_file = open(target, 'rb+')
    source_file = open(source, 'rb')

    pss_mux_from_bytes_io(target_file, source_file)


def pss_mux_from_bytes_io(source_io: io.BytesIO, target_io: io.BytesIO):
    total_buffer_written = 0x0
    source_full_buff = build_full_audio_buffer(source_io)

    seek_next_audio(target_io)
    seek_next_audio(source_io)

    target_total_size, target_curr_block_size = initial_audio_block(target_io)
    source_total_size, source_curr_block_size = initial_audio_block(source_io)

    source_io.seek(source_curr_block_size, 1)
    target_io.write(bytearray(source_full_buff[0:target_curr_block_size]))

    total_buffer_written += target_curr_block_size

    while True:
        target_done = seek_next_audio(target_io)
        source_done = seek_next_audio(source_io)

        if target_done or source_done:
            break

        target_curr_block_size = audio_block(target_io)
        source_curr_block_size = audio_block(source_io)

        source_io.seek(source_curr_block_size, 1)

        end_offset = total_buffer_written + target_curr_block_size
        target_io.write(bytearray(source_full_buff[total_buffer_written:end_offset]))

        total_buffer_written += target_curr_block_size

        if target_curr_block_size < source_curr_block_size:
            print('Smaller block size')
        elif target_curr_block_size > source_curr_block_size:
            print('Bigger block size')

    source_io.close()
    return_value = target_io.read()
    target_io.close()

    return return_value


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description='Inject the audio of a PSS file into an other.')
    parser.add_argument('--source', type=str, required=True,
                        help='PSS source file from which the audio will be taken')

    parser.add_argument('--target', type=str, required=True,
                        help='PSS target file to which the audio will be injected')

    args = parser.parse_args()

    target_extension_index = args.target.rfind('.')

    new_target_filename = args.target[:target_extension_index] + '_mux' + args.target[target_extension_index:]

    copyfile(args.target, new_target_filename)
    pss_mux(args.source, new_target_filename)
