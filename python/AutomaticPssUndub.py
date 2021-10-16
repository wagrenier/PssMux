import argparse
import io
from shutil import copyfile

from python.PssFinder import scan_file_for_pss, extract_file_content
from python.PssMux import pss_mux_from_bytes_io


def prepare_undub(target_filename: str):
    target_extension_index = target_filename.rfind('.')

    new_target_filename = target_filename[:target_extension_index] + '_undub' + target_filename[target_extension_index:]

    copyfile(target_filename, new_target_filename)

    return new_target_filename


def write_file_content_at_address(address, file_content, target_filename):
    with open(target_filename, 'rb+') as target_file:
        target_file.seek(address)
        target_file.write(file_content)


def undub(source_filename: str, target_filename: str):
    target_filename = prepare_undub(target_filename)

    source_video_files = scan_file_for_pss(source_filename)
    target_video_files = scan_file_for_pss(target_filename)

    with open(target_filename, 'rb') as target_binary_file, open(source_filename, 'rb') as source_binary_file:
        i = 0
        for source_video_info in source_video_files:
            source_video_file_content = io.BytesIO(extract_file_content(source_video_info, source_binary_file))
            target_video_file_content = io.BytesIO(extract_file_content(target_video_files[i], target_binary_file))
            undubbed_pss = pss_mux_from_bytes_io(source_video_file_content, target_video_file_content)
            write_file_content_at_address(target_video_files[i]['start'], undubbed_pss, target_filename)
            i += 1


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description='Injects the source\'s video\'s audio stream into the target\'s '
                                                 'video\'s audio stream.')

    parser.add_argument('--source', type=str, required=True,
                        help='ISO source file from which the assets will be taken')

    parser.add_argument('--target', type=str, required=True,
                        help='ISO target file to which the assets will be injected')

    args = parser.parse_args()

    undub(args.source, args.target)
