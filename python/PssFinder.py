# Imports
import argparse
import os

# Global Values
from python.FileOperations import write_buffer_to_file

starting_bytes = b'\x00\x00\x01\xBA\x44\x00\x04\x00\x04\x01\x01\x62\x63\xF8\x00\x00'
ending_bytes = b'\x00\x00\x01\xB9\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00'


def extract_file_content(video_file_info, file_read_stream):
    read_size = video_file_info['end'] - video_file_info['start']

    if read_size <= 0:
        return

    file_read_stream.seek(video_file_info['start'])

    full_buffer = file_read_stream.read(read_size)

    return full_buffer


def extract_files_save_to_file(video_files_found, filename: str):
    with open(filename, 'rb') as binary_file:
        for video_file in video_files_found:
            video_content = extract_file_content(video_file, binary_file)
            write_buffer_to_file(video_content, os.path.dirname(filename), f'{video_file["index"]}.pss')


def scan_file_for_pss(filename: str):
    files_found = []

    with open(filename, 'rb') as binary_file:
        file_index = 0

        current_bytes = binary_file.read(0x10)
        file_found = False
        file_start = 0x0
        while current_bytes:
            if current_bytes.find(starting_bytes) >= 0 and (binary_file.tell() - 0x10) % 0x800 == 0 and not file_found:
                file_start = binary_file.tell() - 0x10
                file_found = True
            if current_bytes.find(ending_bytes) >= 0 and file_found:
                file_found = False
                files_found.append({
                    'index': file_index,
                    'start': file_start,
                    'end': binary_file.tell() - 0xC
                })

                file_index += 1

            current_bytes = binary_file.read(0x10)

    return files_found


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description='Finds PSS files in a bin file.')
    parser.add_argument('--file', type=str, required=True,
                        help='Binary file that will be scanned for PSS files.')

    args = parser.parse_args()

    video_files = scan_file_for_pss(args.file)

    extract_files(video_files, args.file)
