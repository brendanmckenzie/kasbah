import React from 'react'
import PropTypes from 'prop-types'
import { connect } from 'react-redux'
import { NavLink } from 'react-router-dom'
import { actions as contentActions } from 'store/appReducers/content'
import Loading from 'components/Loading'
import ContentEditorForm from 'forms/ContentEditorForm'
import SideBar from './SideBar'

class ContentDetail extends React.Component {
  static propTypes = {
    match: PropTypes.object.isRequired,
    content: PropTypes.object.isRequired,
    getDetail: PropTypes.func.isRequired,
    putDetail: PropTypes.func.isRequired
  }

  componentWillMount() {
    const { match: { params } } = this.props

    this.props.getDetail(params.id)
  }

  componentWillReceiveProps(nextProps) {
    if (nextProps.match.params.id !== this.props.match.params.id) {
      this.props.getDetail(nextProps.match.params.id)
    }
  }

  handleSave = (values) => {
    const { match: { params } } = this.props
    const detail = this.props.content.detail[params.id]

    this.props.putDetail(detail.node.id, values, true)
  }

  get breadcrumb() {
    const { match: { params } } = this.props

    if (!this.props.content.detail[params.id]) {
      return null
    }

    const detail = this.props.content.detail[params.id]
    const { taxonomy, id } = detail.node

    return (<ul className='breadcrumb'>
      {taxonomy.aliases.map((ent, index) => (
        <li key={index}>
          {taxonomy.ids[index] === id
            ? (<span>{ent}</span>)
            : (<NavLink to={`/content/${taxonomy.ids[index]}`}>{ent}</NavLink>)}
        </li>
      ))}
    </ul>)
  }

  render() {
    const { match: { params } } = this.props

    if (!this.props.content.detail[params.id]) {
      return <Loading />
    }

    const detail = this.props.content.detail[params.id]

    // TODO: why is the form not updating

    return (
      <div>
        <h2 className='subtitle'>{detail.node.displayName}</h2>
        {this.breadcrumb}
        <div className='columns'>
          <div className='column'>
            {detail.type.fields.length === 0
              ? <p>This content type does not define any editable fields.</p>
              : <ContentEditorForm
                initialValues={detail.data}
                type={detail.type}
                loading={detail.saving}
                onSubmit={this.handleSave} />}
          </div>
          <div className='column is-3'>
            <SideBar {...detail} />
          </div>
        </div>
      </div>
    )
  }
}

const mapStateToProps = (state, ownProps) => ({
  content: state.content
})

const mapDispatchToProps = {
  ...contentActions
}

export default connect(mapStateToProps, mapDispatchToProps)(ContentDetail)
