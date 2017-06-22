import React from 'react'
import PropTypes from 'prop-types'
import moment from 'moment'

export const Information = ({ node, type, data, onDelete, onRename, onChangeType, onMove }) => (
  <div className='content'>
    <p>
      <span className='heading'>Type</span>
      <span>{type.displayName}</span>
    </p>

    <p>
      <span className='heading'>Version</span>
      <span>{data ? data['_version'] : 'new'}</span>
    </p>

    <p>
      <span className='heading'>Created</span>
      <span>{moment(node.created).fromNow()}</span>
    </p>
    <p>
      <span className='heading'>Modified</span>
      <span>{moment(node.modified).fromNow()}</span>
    </p>
    <p>
      <span className='heading'>Status</span>
      <span>{node.publishedVersion ? `Published (v${node.publishedVersion})` : 'Draft'}</span>
    </p>

    <div className='control has-addons is-fullwidth'>
      <button className='button is-danger is-small' type='button' onClick={onDelete}>Delete</button>
      <button className='button is-small' type='button' onClick={onRename}>Rename</button>
      <button className='button is-small' type='button' onClick={onChangeType}>Change type</button>
      <button className='button is-small' type='button' onClick={onMove}>Move</button>
    </div>
  </div>
)

Information.propTypes = {
  node: PropTypes.object.isRequired,
  type: PropTypes.object.isRequired,
  data: PropTypes.object,
  onDelete: PropTypes.func.isRequired,
  onRename: PropTypes.func.isRequired,
  onChangeType: PropTypes.func.isRequired,
  onMove: PropTypes.func.isRequired
}

export default Information
