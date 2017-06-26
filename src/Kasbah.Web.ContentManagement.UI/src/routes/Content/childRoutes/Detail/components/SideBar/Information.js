import React from 'react'
import PropTypes from 'prop-types'
import moment from 'moment'

export const Information = ({ node, type, data, onDelete, onRename, onEdit, onMove }) => (
  <div className='content'>
    <p>
      <span className='heading'>Alias</span>
      <span>{node.alias}</span>
    </p>

    <p>
      <span className='heading'>Display name</span>
      <span>{node.displayName}</span>
    </p>

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

    <div className='field is-grouped'>
      <div className='control'>
        <button className='button is-danger' type='button' onClick={onDelete}>
          <span className='icon is-small'><i className='fa fa-trash' /></span>
          <span>Delete</span>
        </button>
      </div>
      <div className='control'>
        <button className='button' type='button' onClick={onEdit}>
          <span className='icon is-small'><i className='fa fa-pencil' /></span>
          <span>Edit</span>
        </button>
      </div>
      <div className='control'>
        <button className='button' type='button' onClick={onMove}>
          <span className='icon is-small'><i className='fa fa-long-arrow-right' /></span>
          <span>Move</span>
        </button>
      </div>
    </div>
  </div>
)

Information.propTypes = {
  node: PropTypes.object.isRequired,
  type: PropTypes.object.isRequired,
  data: PropTypes.object,
  onDelete: PropTypes.func.isRequired,
  onEdit: PropTypes.func.isRequired,
  onMove: PropTypes.func.isRequired
}

export default Information
