import React from 'react'
import moment from 'moment'

export const Information = ({ node, type, data, onDelete, onRename, onChangeType }) => (
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
    </div>
  </div>
)

Information.propTypes = {
  node: React.PropTypes.object.isRequired,
  type: React.PropTypes.object.isRequired,
  data: React.PropTypes.object,
  onDelete: React.PropTypes.func.isRequired,
  onRename: React.PropTypes.func.isRequired,
  onChangeType: React.PropTypes.func.isRequired
}

export default Information
