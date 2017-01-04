import React from 'react'
import moment from 'moment'

export const Information = ({ node, type, data }) => (
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
  </div>
)

Information.propTypes = {
  node: React.PropTypes.object.isRequired,
  type: React.PropTypes.object.isRequired,
  data: React.PropTypes.object.isRequired
}

export default Information
