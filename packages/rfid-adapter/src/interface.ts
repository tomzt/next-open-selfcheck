// SPDX-License-Identifier: GPL-3.0-or-later
// open-selfcheck — rfid-adapter interface
// All RFID drivers must implement this interface.

export interface TagData {
  /** ISO 15693 UID */
  tagId: string
  /** Application Family Identifier */
  afi: number
  /** Optional raw block data */
  rawData?: Uint8Array
}

export interface TagProgramData {
  /** Library item barcode (from ILS lookup or manual entry) */
  itemBarcode: string
  /** Deployer's library identifier — set via LIBRARY_CODE env */
  libraryCode: string
  /** Optional extra data blocks */
  customBlocks?: Record<number, Uint8Array>
}

export interface RFIDAdapter {
  /** Connect to the RFID reader */
  connect(): Promise<void>
  /** Disconnect from the RFID reader */
  disconnect(): void
  /** Read a tag — resolves when a tag is detected */
  readTag(): Promise<TagData>
  /** Write AFI byte to a tag */
  writeAFI(tagId: string, afi: number): Promise<void>
  /** Write full program data to a tag (item barcode + library code) */
  writeProgramData(tagId: string, data: TagProgramData): Promise<void>
}

/** AFI values per ISO 15693 library standard */
export const AFI = {
  /** Item is in library — available or returned */
  IN_LIBRARY: 0x07,
  /** Item is checked out */
  CHECKED_OUT: 0x02,
} as const
