import React from 'react';
import { renderLayout, Layout } from './../layout/index';
import { shallow, configure } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import ShallowRenderer from 'react-test-renderer/shallow';
import renderer from 'react-test-renderer';

configure({ adapter: new Adapter() });

describe('Layout', () => {
    describe('render', () => {
        it('match snapshots', () => {
            // Act
            const component = renderer.create(<Layout>{"Test"}</Layout>);
            // Assert
            expect(component.toJSON()).toMatchSnapshot();
        });

        it('match snapshots with child component', () => {
            let children = <div>Test</div>;
            const component = shallow(
                <Layout>
                    {children}
                </Layout>
            );
            expect(component.contains(children)).toBeTruthy();
        });
    });
});