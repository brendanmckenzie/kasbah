import React from 'react'
import PropTypes from 'prop-types'
import { connect } from 'react-redux'
import { actions as contentActions } from 'store/appReducers/content'
import Loading from 'components/Loading'
import ContentEditorForm from 'forms/ContentEditorForm'
import SideBar from './SideBar'
import Breadcrumbs from './Breadcrumbs'
import { Columns, Column, Field } from 'components/Layout'

class ContentDetail extends React.Component {
  static propTypes = {
    match: PropTypes.object.isRequired,
    content: PropTypes.object.isRequired,
    getDetail: PropTypes.func.isRequired,
    putDetail: PropTypes.func.isRequired,
  }

  componentDidMount() {
    const {
      match: { params },
    } = this.props

    this.props.getDetail(params.id)
  }

  componentDidUpdate(prevProps) {
    if (prevProps.match.params.id !== this.props.match.params.id) {
      this.props.getDetail(this.props.match.params.id)
    }
  }

  handleSave = (values) => {
    const {
      match: { params },
    } = this.props
    const detail = this.props.content.detail[params.id]

    this.props.putDetail(detail.node.id, values, true)
  }

  render() {
    const {
      match: { params },
    } = this.props
    const detail = this.props.content.detail[params.id]

    if (!detail) {
      return <Loading />
    }

    return (
      <div>
        <h2 className="subtitle">{detail.node.displayName}</h2>
        <Field>
          <Breadcrumbs id={params.id} content={this.props.content} />
        </Field>

        <Columns>
          <Column>
            {detail.type.fields.length === 0 ? (
              <p>This content type does not define any editable fields.</p>
            ) : (
              <ContentEditorForm
                initialValues={detail.data}
                type={detail.type}
                loading={detail.saving}
                onSubmit={this.handleSave}
              />
            )}
          </Column>
          <Column className="is-3">
            <SideBar {...detail} />
          </Column>
        </Columns>
      </div>
    )
  }
}

const mapStateToProps = (state, ownProps) => ({
  content: state.content,
})

const mapDispatchToProps = {
  ...contentActions,
}

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(ContentDetail)
