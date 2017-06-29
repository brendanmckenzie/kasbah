import React from 'react'
import PropTypes from 'prop-types'
import moment from 'moment'
import DeleteButton from './DeleteButton'
import EditButton from './EditButton'
import MoveButton from './MoveButton'

const Information = ({ node, type, data }) => (
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
      <span>{moment.utc(node.created).fromNow()}</span>
    </p>
    <p>
      <span className='heading'>Modified</span>
      <span>{moment.utc(node.modified).fromNow()}</span>
    </p>
    <p>
      <span className='heading'>Status</span>
      <span>{node.publishedVersion ? `Published (v${node.publishedVersion})` : 'Draft'}</span>
    </p>

    <div className='field is-grouped'>
      <div className='control'>
        <DeleteButton node={node} />
      </div>
      <div className='control'>
        <EditButton node={node} />
      </div>
      <div className='control'>
        <MoveButton node={node} />
      </div>
    </div>
  </div >

)

Information.propTypes = {
  node: PropTypes.object.isRequired,
  type: PropTypes.object.isRequired,
  data: PropTypes.object
}

export default Information
