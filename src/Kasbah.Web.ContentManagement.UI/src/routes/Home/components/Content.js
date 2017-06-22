import React from 'react'
import PropTypes from 'prop-types'
import moment from 'moment'
import { Link } from 'react-router'
import Loading from 'components/Loading'
import Error from 'components/Error'

class Content extends React.Component {
  static propTypes = {
    listLatestUpdatesRequest: PropTypes.func.isRequired,
    listLatestUpdates: PropTypes.object.isRequired
  }

  componentWillMount() {
    this.props.listLatestUpdatesRequest({ take: 7 })
  }

  get list() {
    if (this.props.listLatestUpdates.loading) {
      return <Loading />
    }

    if (!this.props.listLatestUpdates.success) {
      return <Error />
    }

    return (
      this.props.listLatestUpdates.payload.map(ent => (
        <div key={ent.id} className='level'>
          <div className='level-left level-shrink'>
            <div className='level-item level-shrink'>
              <p><Link to={`/content/${ent.id}`}>{ent.displayName || ent.alias}</Link></p>
            </div>
          </div>
          <div className='level-right'>
            <p className='level-item'>
              <small title={moment.utc(ent.modified).format('MMM Do h:mma')}>{moment(ent.modified).fromNow()}</small>
            </p>
          </div>
        </div>
      ))
    )
  }

  render() {
    return (
      (
        <div className='tile is-child notification is-success'>
          <h2 className='title'><Link to='/content'>Recent updates</Link></h2>
          {this.list}
        </div>
      )
    )
  }
}

export default Content
